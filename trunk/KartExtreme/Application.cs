using KartExtreme.IO;
using KartExtreme.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KartExtreme
{
    internal static class Application
    {
        private static bool _isRunning = false;

        [STAThread]
        private static void Main(string[] args)
        {
            Log.Entitle("KartExtreme");

            DateTime dt = DateTime.Now;

            try
            {
                Server.Initialize();

                Application._isRunning = true;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (Application._isRunning)
            {
                Log.Inform("Initialized in {0}ms.", (DateTime.Now - dt).TotalMilliseconds);
            }
            else
            {
                Log.Inform("Unable to initialize.");
            }

            Log.SkipLine();

            while (Application._isRunning)
            {
                Console.Read();
            }

            Log.Quit();
        }
    }
}
