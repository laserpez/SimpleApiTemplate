using System;
using Microsoft.Owin.Hosting;

namespace ConfigApi
{
    internal class Program
    {
        private static void Main()
        {
            using (WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.ReadLine();
            }
        }
    }
}
