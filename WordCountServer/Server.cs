using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WordCountServer
{
    class Server
    {
        private HttpListener _listener;
        private string _prefix;
        private bool _running;
        private FileSearcher _fileSearcher;
        private RequestQueue _requestQueue;

        private int _workerCount = 4; //br niti koje ce obraditi zahteve

        public Server(string prefix, string rootFolder)
        {
            _prefix = prefix;
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _fileSearcher = new FileSearcher(rootFolder);

            _requestQueue = new RequestQueue(10);
        }

        public void Start()
        {
            _listener.Start();
            _running = true;
            Console.WriteLine($"server pokrenut na: {_prefix}");

            for (int i = 0; i < _workerCount; i++)
            {
                int workerId = i;
                ThreadPool.QueueUserWorkItem(_ => WorkerLoop(workerId));
            }

            Console.WriteLine($"pokrenuto je {_workerCount} worker niti");

            while (_running)
            {
                HttpListenerContext context = _listener.GetContext();
                Console.WriteLine("nova konekcija primljena, dodaje se u red");
                _requestQueue.Enqueue(context);
            }
        }

        private void WorkerLoop(int workerId)
        {
            Console.WriteLine($"worker {workerId} spreman");

            while (_running)
            {
                // uzima sl zahtev(blokira ako nema nista)
                HttpListenerContext context = _requestQueue.Dequeue();

                string rawUrl = context.Request.Url.AbsolutePath;
                string fileName = rawUrl.TrimStart('/');

                Console.WriteLine($"worker {workerId} obradjuje: {fileName}");

                string response = ProcessRequest(fileName);
                SendResponse(context, response);

                Console.WriteLine($"worker {workerId} zavrsio obradu: {fileName}");
            }
        }

        private string ProcessRequest(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "greska! nije naveden naziv fajla";

            string fullPath = _fileSearcher.FindFile(fileName);

            if (fullPath == null)
                return $"greska! fajl '{fileName}' nije pronadjen";

            string content = _fileSearcher.ReadFile(fullPath);
            int count = WordCounter.CountWords(content);

            if (count == 0)
                return $"fajl '{fileName}' ne sadrzi reci sa vise suglasnika nego samoglasnika";

            return $"fajl: {fileName}\n putanja: {fullPath}\nbroj reci: {count}";
        }

        public void Stop()
        {
            _running = false;
            _listener.Stop();
        }

        private void SendResponse(HttpListenerContext context, string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.StatusCode = 200;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}
