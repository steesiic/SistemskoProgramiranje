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
            //folderi za primere i logove
            /*jodza:
                    @"C:\Users\Jetko\Desktop\FAKS\III godina\Sistemsko\SistemskoProgramiranje\WordCountFiles";  
                    @"C:\Users\Jetko\Desktop\FAKS\III godina\Sistemsko\SistemskoProgramiranje\Logs";*/
            /*nidza:
                    @"D:\faks\TRECA GODINA\sistemsko programiranje\Projekat\WordCountFiles";
                    @"D:\faks\TRECA GODINA\sistemsko programiranje\Projekat\Logs";*/
            string rootFolder = @"D:\faks\TRECA GODINA\sistemsko programiranje\Projekat\WordCountFiles"; 
            string logFolder = @"D:\faks\TRECA GODINA\sistemsko programiranje\Projekat\Logs";

            Logger.Init(logFolder);
            Logger.Info("Aplikacija pokrenuta.");

            Server server = new Server("http://localhost:5050/", rootFolder);
            server.Start();

            Console.ReadLine();
            server.Stop();
            Logger.Info("Aplikacija zaustavljena.");
        }
    }
}
