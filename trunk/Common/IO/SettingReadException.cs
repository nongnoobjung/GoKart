using System;

namespace KartExtreme.IO
{
    public class SettingReadException : Exception
    {
        public SettingReadException(string key) : base(string.Format("Failed to read setting '{0}'.", key)) { }
    }
}
