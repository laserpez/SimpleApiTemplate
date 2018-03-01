using System;
using Microsoft.Owin.Hosting;
using ProjectAPI;

namespace ConsoleHostProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.ReadLine();
            }
        }
    }
}
