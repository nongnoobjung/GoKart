namespace KartExtreme
{
    public static class Constants
    {
        public static readonly Version Version = new Version()
        {
            Localisation = 5002,
            Major = 1,
            Minor = 26
        };
    }

    public struct Version
    {
        public short Localisation { get; set; }
        public short Major { get; set; }
        public short Minor { get; set; }
    }
}
