using System;
using Microsoft.Owin.Hosting;
using ProjectAPI;

namespace ConsoleHostProject
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
