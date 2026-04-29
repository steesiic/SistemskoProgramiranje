using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCountServer
{
    class FileSearcher
    {
        private string _rootFolder;

        public FileSearcher(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public string FindFile(string fileName)
        {
            string[] foundFiles = Directory.GetFiles(
                _rootFolder,
                fileName,
                SearchOption.AllDirectories
            );

            if (foundFiles.Length == 0)
                return null; 

            
            return foundFiles[0]; //ako ima fajlova sa istim imenom, uzima se prvi
        }

        public string ReadFile(string fullPath)
        {
            return File.ReadAllText(fullPath, System.Text.Encoding.UTF8);
        }
    }
}
