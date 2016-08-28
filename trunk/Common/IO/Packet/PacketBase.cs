using System.IO;
using System.Text;

namespace KartExtreme.IO.Packet
{
    public abstract class PacketBase
    {
        public abstract int Length { get; }
        public abstract int Position { get; }

        public abstract byte[] ToArray();

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (byte b in this.ToArray())
            {
                sb.AppendFormat("{0:X2} ", b);
            }

            return sb.ToString();
        }
    }
}
