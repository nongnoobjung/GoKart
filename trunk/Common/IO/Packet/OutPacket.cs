using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace KartExtreme.IO.Packet
{
    public class OutPacket : PacketBase, IDisposable
    {
        private MemoryStream m_stream;
        private bool m_disposed;

        public override int Length
        {
            get { return (int)m_stream.Position; }
        }
        public override int Position
        {
            get { return (int)m_stream.Position; }
        }

        public bool Disposed
        {
            get
            {
                return m_disposed;
            }
        }

        public OutPacket(int size = 64)
        {
            m_stream = new MemoryStream(size);
            m_disposed = false;
        }

        //From LittleEndianByteConverter by Shoftee
        private void Append(long value, int byteCount)
        {
            for (int i = 0; i < byteCount; i++)
            {
                m_stream.WriteByte((byte)value);
                value >>= 8;
            }
        }

        public void WriteBool(bool value)
        {
            ThrowIfDisposed();
            WriteByte(value ? (byte)1 : (byte)0);
        }

        public void WriteByte(byte value = 0)
        {
            ThrowIfDisposed();
            m_stream.WriteByte(value);
        }
        public void WriteSByte(sbyte value = 0)
        {
            WriteByte((byte)value);
        }

        public void WriteBytes(params byte[] value)
        {
            ThrowIfDisposed();
            m_stream.Write(value, 0, value.Length);
        }

        public void WriteShort(short value = 0)
        {
            ThrowIfDisposed();
            Append(value, 2);
        }
        public void WriteUShort(ushort value = 0)
        {
            WriteShort((short)value);
        }

        public void WriteInt(int value = 0)
        {
            ThrowIfDisposed();
            Append(value, 4);
        }
        public void WriteUInt(uint value = 0)
        {
            WriteInt((int)value);
        }

        public void WriteLong(long value = 0)
        {
            ThrowIfDisposed();
            Append(value, 8);
        }
        public void WriteULong(ulong value = 0)
        {
            WriteLong((long)value);
        }

        public void WriteString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            WriteInt(value.Length);
            WriteString(value, value.Length);
        }
        public void WriteString(string value, int length)
        {
            if (value == null   ||
                length < 1      ||
                length > value.Length)

                throw new ArgumentNullException("value");

            var bytes = Encoding.Unicode.GetBytes(value);
            var i = 0;

            for (; i < value.Length & i < length; i++)
            {
                var offset = i * 2;
                this.WriteByte(bytes[offset]);
                this.WriteByte(bytes[offset + 1]);
            }

            for (; i < length; i++)
            {
                this.WriteShort();
            }
        }

        public void WriteHexString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            value = value.Replace(" ", "");

            for (int i = 0; i < value.Length; i += 2)
            {
                WriteByte(byte.Parse(value.Substring(i, 2),  NumberStyles.HexNumber));
            }
        }

        private void ThrowIfDisposed()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public override byte[] ToArray()
        {
            ThrowIfDisposed();
            return m_stream.ToArray();
        }

        public void Dispose()
        {
            m_disposed = true;

            if (m_stream != null)
                m_stream.Dispose();

            m_stream = null;
        }
    }
}
