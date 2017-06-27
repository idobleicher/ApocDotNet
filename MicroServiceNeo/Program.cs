using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServiceNeo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose A port to run(at localhost)");
            string baseAddressPort = Console.ReadLine();

            string baseAddress = "http://localhost:" + baseAddressPort + "/";

            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.WriteLine("Started Midur service....");
                Console.WriteLine($"Listening to address: { baseAddress } ");
                Console.ReadLine();
            }
        }
    }
}
