using System;

namespace KartExtreme.Net
{
    public sealed class SendPacketException : Exception
    {
        public SendPacketException()
            : base("Disconnected while sending packet")
        {
        }
    }
}
