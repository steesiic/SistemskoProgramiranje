using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WordCountServer
{
    class RequestQueue
    {
        private Queue<HttpListenerContext> _queue = new Queue<HttpListenerContext>();

        private int _maxSize; //maks zahteva koji mogu cekati

        private object _lock = new object();

        public RequestQueue(int maxSize)
        {
            _maxSize = maxSize;
        }
        public void Enqueue(HttpListenerContext context)
        {
            lock (_lock)
            {
                while (_queue.Count >= _maxSize)
                {
                    Console.WriteLine("red je pun! ceka se");
                    Monitor.Wait(_lock);
                }

                _queue.Enqueue(context);
                Console.WriteLine($"dodat zahtev u red, sada ih je: {_queue.Count}");

                Monitor.Pulse(_lock);
            }
        }

        public HttpListenerContext Dequeue()
        {
            lock (_lock)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_lock);
                }

                HttpListenerContext context = _queue.Dequeue();
                Console.WriteLine($"zahtev izvucen iz reda, sada ih je: {_queue.Count}");

                Monitor.Pulse(_lock);

                return context;
            }
        }
    }
}
