using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCountServer
{
    class CacheEntry
    {
        public int Result { get; set; }    
        public bool IsReady { get; set; } 

        public CacheEntry(int result)
        {
            Result = result;
            IsReady = true;
        }
        public CacheEntry()
        {
            IsReady = false;
        }
    }
}
