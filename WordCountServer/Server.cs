using System;
using System.Net;
using System.Threading;

namespace WordCountServer
{
    class Server
    {
        private HttpListener _listener;
        private string _prefix;
        private bool _running;
        private FileSearcher _fileSearcher;
        private RequestQueue _requestQueue;
        private Cache _cache;
        private int _workerCount = 4;

        public Server(string prefix, string rootFolder)
        {
            _prefix = prefix;
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _fileSearcher = new FileSearcher(rootFolder);
            _requestQueue = new RequestQueue(10);
            _cache = new Cache(5);
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
                _requestQueue.Enqueue(context);
            }
        }

        private void WorkerLoop(int workerId)
        {
            Console.WriteLine($"worker {workerId} spreman");

            while (_running)
            {
                HttpListenerContext context = _requestQueue.Dequeue();

                string rawUrl = context.Request.Url.AbsolutePath;
                string fileName = rawUrl.TrimStart('/');

                if (fileName == "favicon.ico")
                {
                    context.Response.StatusCode = 404;
                    context.Response.OutputStream.Close();
                    continue;
                }

                Console.WriteLine($"worker {workerId} obradjuje: {fileName}");
                string response = ProcessRequest(fileName);
                SendResponse(context, response);
                Console.WriteLine($"worker {workerId} zavrsio: {fileName}");
            }
        }

        private string ProcessRequest(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "greska! nije naveden naziv fajla";

            if (_cache.TryGetOrReserve(fileName, out int cachedResult))
            {
                if (cachedResult == -1)
                    return $"greska! fajl '{fileName}' nije pronadjen";
                if (cachedResult == 0)
                    return $"fajl '{fileName}' ne sadrzi reci sa vise suglasnika nego samoglasnika";
                return $"fajl: {fileName}\nbroj reci (iz kesa): {cachedResult}";
            }

            // Thread.Sleep(3000); // odkomentarisi za testiranje cache stampede

            string fullPath = _fileSearcher.FindFile(fileName);

            if (fullPath == null)
            {
                _cache.Set(fileName, -1);
                return $"greska! fajl '{fileName}' nije pronadjen";
            }

            string content = _fileSearcher.ReadFile(fullPath);
            int count = WordCounter.CountWords(content);

            _cache.Set(fileName, count);

            if (count == 0)
                return $"fajl '{fileName}' ne sadrzi reci sa vise suglasnika nego samoglasnika";

            return $"fajl: {fileName}\nputanja: {fullPath}\nbroj reci: {count}";
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