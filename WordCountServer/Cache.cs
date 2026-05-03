using System;
using System.Collections.Generic;
using System.Net;

namespace WordCountServer
{
    class Cache
    {
        private Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();
        private LinkedList<string> _lruList = new LinkedList<string>();
        private int _maxSize;
        private object _lock = new object();

        public Cache(int maxSize)
        {
            _maxSize = maxSize;
        }

        public bool TryGetOrReserve(string fileName, out int result)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(fileName, out CacheEntry entry))
                {
                    while (!entry.IsReady)
                    {
                        Logger.Cache($"'{fileName}' se vec obradjuje, sacekaj!");
                        Monitor.Wait(_lock);
                    }

                    _lruList.Remove(fileName);
                    _lruList.AddLast(fileName);

                    result = entry.Result;
                    Logger.Cache($"pogodak za '{fileName}' = {result}");
                    return true;
                }

                if (_cache.Count >= _maxSize)
                    EvictLRU();

                _cache[fileName] = new CacheEntry();
                _lruList.AddLast(fileName);

                result = 0;
                return false;
            }
        }

        public void Set(string fileName, int result)
        {
            lock (_lock)
            {
                if (_cache.ContainsKey(fileName))
                {
                    _cache[fileName].Result = result;
                    _cache[fileName].IsReady = true;
                }
                else
                {
                    if (_cache.Count >= _maxSize)
                        EvictLRU();

                    _cache[fileName] = new CacheEntry(result);
                    _lruList.AddLast(fileName);
                }

                Monitor.PulseAll(_lock);
            }
        }

        private void EvictLRU()
        {
            string oldest = _lruList.First.Value;
            _lruList.RemoveFirst();
            _cache.Remove(oldest);
            Logger.Cache($"izbacen '{oldest}' (LRU)");
        }
    }
}