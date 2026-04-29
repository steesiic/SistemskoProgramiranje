using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCountServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootFolder = @"D:\faks\TRECA GODINA\sistemsko programiranje\Projekat\WordCountFiles";  //folder sa primerima

            Server server = new Server("http://localhost:5050/", rootFolder);
            server.Start();

            Console.WriteLine("pritisnite enter za kraj");
            Console.ReadLine();

            server.Stop();
        }
    }
}
