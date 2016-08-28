using KartExtreme.IO;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using KartExtreme.IO.Packet;

namespace KartExtreme.Net
{
    public abstract class Session
    {
        private Socket _socket;

        private byte[] _buffer;
        private int _bufferIndex;

        private bool _header;
        private bool _connected;

        private string _label;

        private uint riv;
        private uint siv;

        private object _lock;

        public string Label
        {
            get
            {
                return _label;
            }
        }

        public bool IsConnected
        {
            get
            {
                return _connected;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Session class.
        /// </summary>
        /// <param name="socket"></param>
        public Session(Socket socket)
        {
            this._socket = socket;
            this._socket.NoDelay = true;

            this._label = this._socket.RemoteEndPoint.ToString();

            this._connected = true;

            this._lock = new object();

            this.InitiateReceive(4, true);
        }

        /// <summary>
        /// Initiates the receiving mechanism.
        /// </summary>
        /// <param name="length">The length of the data</param>
        /// <param name="header">Indicates if a header is received</param>
        private void InitiateReceive(uint length, bool header = false)
        {
            if (!this._connected)
            {
                return;
            }

            this._header = header;
            this._buffer = new byte[length];
            this._bufferIndex = 0;

            this.BeginReceive();
        }

        /// <summary>
        /// Begins to asynchronously receive data from the socket.
        /// </summary>
        private void BeginReceive()
        {
            if (!this._connected)
            {
                return;
            }

            var error = SocketError.Success;

            this._socket.BeginReceive(this._buffer,
                this._bufferIndex,
                this._buffer.Length - this._bufferIndex,
                SocketFlags.None, out error,
                EndReceive,
                null);

            if (error != SocketError.Success)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Reads the data from the callback and handles it.
        /// </summary>
        /// <param name="iar"></param>
        private void EndReceive(IAsyncResult iar)
        {
            if (!this._connected)
            {
                return;
            }

            var error = SocketError.Success;
            int received = this._socket.EndReceive(iar, out error);

            if (received == 0 || error != SocketError.Success)
            {
                this.Close();
                return;
            }

            this._bufferIndex += received;

            if (this._bufferIndex == this._buffer.Length)
            {
                if (this._header)
                {
                    uint header = BitConverter.ToUInt32(this._buffer, 0);

                    if(riv != 0)
                        header = (riv ^ header ^ 0xF834A608);

                    this.InitiateReceive(header, false);
                }
                else
                {
                    if (riv != 0)
                    {
                        int bufferLength = _buffer.Length - 4;

                        uint originalChecksum = (uint)(riv ^ BitConverter.ToUInt32(this._buffer, bufferLength) ^ 0xC9F84A90);

                        Array.Resize<byte>(ref this._buffer, bufferLength);

                        uint checksum = Cryptography.HashPacket(this._buffer, riv, false, (uint)bufferLength);

                        if (originalChecksum != checksum)
                        {
                            Log.Warn("Decrypt Checksum different!");
                        }

                        riv += 21446425;

                        if (riv == 0)
                            riv = 1;
                    }

                    this.OnPacket(new InPacket(this._buffer));
                    this.InitiateReceive(4, true);
                }
            }
            else
            {
                this.BeginReceive();
            }
        }

        /// <summary>
        /// Sends the initial patch data packet to the socket.
        /// </summary>
        /// <param name="version">The version's information</param>
        /// <param name="patchLocation">The patch data URL</param>
        public void SendPatchData(Version version, string patchLocation)
        {
            //Fuck!!!!!
            Random rnd = new Random();
            
            uint val_first = 0;//(uint)rnd.Next();
            uint val_second = 0;//(uint)rnd.Next() ;

            using (OutPacket outPacket = new OutPacket())
            {
                outPacket.WriteHexString("80 05 2B 28"); // NOTE: Header. Do not change. Probably typeid() of handler.
                outPacket.WriteShort(version.Localisation);
                outPacket.WriteShort(version.Major);
                outPacket.WriteShort(version.Minor);
                outPacket.WriteString(patchLocation);
                outPacket.WriteInt((int)val_first); // NOTE: IV Keys. Should we do random stuffs?
                outPacket.WriteInt((int)val_second);
                outPacket.WriteByte(0xE0); // NOTE: Some stuff used on Region / Service Area.
                outPacket.WriteHexString("03 00 09 03 00 00 03 00 00 00 00 00 83 A3 E5 47 00 00 15 00 0D 00 00 00 03 00 00 00 00 00 65");

                this.Send(outPacket);
            }

            riv = val_first ^ val_second;
            siv = val_first ^ val_second;
        }

        /// <summary>
        /// Sends a KartExtreme.IO.OutPacket array to the socket.
        /// </summary>
        /// <param name="outPacket"></param>
        public void Send(OutPacket outPacket)
        {
            this.Send(outPacket.ToArray());
        }

        /// <summary>
        /// Sends data to the socket.
        /// </summary>
        /// <param name="buffer"></param>
        public void Send(byte[] buffer)
        {
            if (!this._connected)
            {
                return;
            }

            lock (_lock)
            {

                byte[] data = new byte[buffer.Length + (siv != 0 ? 8 : 4)];

                if (siv == 0)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes((int)buffer.Length), 0, data, 0, 4);
                }
                else
                {
                    uint newHash = Cryptography.HashPacket(buffer, siv, true, (uint)buffer.Length);

                    // NOTE: Length at beginning.
                    Buffer.BlockCopy(BitConverter.GetBytes((int)(siv ^ (buffer.Length + 4) ^ 0xF834A608)), 0, data, 0, 4);
                    // NOTE: Checksum at end.
                    Buffer.BlockCopy(BitConverter.GetBytes((uint)(siv ^ newHash ^ 0xC9F84A90)), 0, data, data.Length - 4, 4);

                    siv += 0x1473F19;

                    if (siv == 0)
                        siv = 1;
                }

                Buffer.BlockCopy(buffer, 0, data, 4, buffer.Length);

                this.SendRaw(data);
            }
        }

        private void SendRaw(byte[] data)
        {
            if (!this._connected)
            {
                return;
            }

            int offset = 0;

            while (offset < data.Length)
            {
                SocketError error = SocketError.Success;
                int sent = this._socket.Send(data, offset, data.Length - offset, SocketFlags.None, out error);

                if (sent == 0 || error != SocketError.Success)
                {
                    throw new SendPacketException();
                }

                offset += sent;
            }
        }

        /// <summary>
        /// Closes the socket.
        /// </summary>
        public void Close()
        {
            if (!this._connected)
            {
                return;
            }

            this._socket.Shutdown(SocketShutdown.Both);
            this._socket.Close();

            this.OnDisconnect();
        }

        public abstract void OnDisconnect();
        public abstract void OnPacket(InPacket inPacket);
    }
}
