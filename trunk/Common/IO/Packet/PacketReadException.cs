using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KartExtreme.IO.Packet
{
    public sealed class PacketReadException : Exception
    {
        public PacketReadException(string message)
            : base(message)
        {
        }
    }
}
