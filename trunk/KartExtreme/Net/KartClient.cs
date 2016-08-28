using KartExtreme.IO;
using System;
using System.Net.Sockets;
using System.Text;
using KartExtreme.IO.Packet;

namespace KartExtreme.Net
{
    public sealed class KartClient : Session
    {
        public KartClient(Socket socket)
            : base(socket)
        {
            this.SendPatchData(Constants.Version, "http://kart.dn.nexoncdn.co.kr/patch");

            /*
            using (OutPacket outPacket = new OutPacket())
            {
                outPacket.WriteHexString("80 05 2B 28"); // NOTE: HEADER. DO NOT CHANGE. PROBABLY typeid() OF HANDLER?
                // 8
                outPacket.WriteShort(5002); // Note: Localisation.
                // 10
                outPacket.WriteShort(1); // NOTE: Service area/version.
                // 12
                outPacket.WriteShort(26); // NOTE: Subversion number? Current = 26. 25 = Incompatible with server, 27 = Close client.
                // 14
                outPacket.WriteString("http://kart.dn.nexoncdn.co.kr/patch");
                // 18

                // outPacket.WriteHexString("81 82 83 84 85 86 87 88");
                // outPacket.WriteHexString("94 2E F9 A8 E1 4B 13 5D");
                // outPacket.WriteHexString("01 02 03 04 05 06 07 08");

                outPacket.WriteInt(0x11111111);
                outPacket.WriteInt(0x22222222);
                outPacket.WriteByte(0xE0);
                outPacket.WriteHexString("03 00 09 03 00 00 03 00 00 00 00 00 83 A3 E5 47 00 00 15 00 0D 00 00 00 03 00 00 00 00 00 65");
                // NOTE: Nothing more needed.

                if (false)
                {
                    outPacket.WriteHexString("80 05 2B 28"); // NOTE: HEADER. DO NOT CHANGE. PROBABLY typeid() OF HANDLER?
                    outPacket.WriteHexString("EA 00 03 00 95 0B");
                    outPacket.WriteString("");
                    // NOTE: Nothing more needed.

                }

                if (false)
                {
                    outPacket.WriteHexString("80 05 2B 28 EA 03 03 00 95 0B");
                    outPacket.WriteString("");
                    // NOTE: Nothing more needed.
                }
                if (false)
                {
                    // NOTE: Original (Korean KartRider).
                    outPacket.WriteHexString("80 05 2B 28");
                    outPacket.WriteHexString("EA 03 03 00 95 0B");
                    outPacket.WriteString("http://kart.dn.nexoncdn.co.kr/patch");
                    outPacket.WriteHexString("94 2E F9 A8 E1 4B 13 5D 76 03 00 09 03 00 00 03 00 00 00 00 00 83 A3 E5 47 00 00 15 00 0D 00 00 00 03 00 00 00 00 00 65");
                }

                this.Send(outPacket);
            }
             */
        }

        public override void OnDisconnect()
        {
            Log.Inform("Lost connection from {0}.", this.Label);

            Server.RemoveClient(this);
        }

        public override void OnPacket(InPacket inPacket)
        {

            try
            {

                //int typeId = inPacket.ReadInt();
                //byte wdf = BitConverter.

                Log.Hex("Received Packet ", inPacket.ToArray());
                //Log.Inform("String val:" + Encoding.Unicode.GetString(inPacket.ToArray()));

                byte typeId = inPacket.ReadByte();
                byte a2 = inPacket.ReadByte();
                byte a3 = inPacket.ReadByte();
                byte a4 = inPacket.ReadByte();

                int header = BitConverter.ToInt32(new byte[] { typeId, a2, a3, a4 }, 0);
                switch (typeId)
                {
                    case 0xBA:
                        using (OutPacket oPacket = new OutPacket())
                        {
                            oPacket.WriteInt(header + 1); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            oPacket.WriteInt(0); // header
                            Send(oPacket);
                            //WALAO!
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (SendPacketException)
            {
                Log.Error("Client was disconnected while sending packet");
                Close();
            }
            catch (Exception e)
            {
                Log.Error("Exception during packet handling from {0}: {1}", this.Label, e.ToString());
            }
        }
    }
}
