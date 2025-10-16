using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Red.Types
{
    public class Package : IDisposable
    {
        private readonly ZipArchive file;

        private Package()
        {
        }

        public void Dispose()
        {
            if (file != null)
                file.Dispose();
        }

        public Package(string filename)
        {
            file = ZipFile.Open(filename, ZipArchiveMode.Read);
        }

        internal Package(string filename, bool create)
        {
            if (create)
            {
                file = ZipFile.Open(filename, ZipArchiveMode.Create);
                return;
            }

            file = ZipFile.Open(filename, ZipArchiveMode.Update);
        }

        public static Package Create(string filename)
        {
            return new Package(filename, true);
        }
        public async Task<MemoryStream> GetEntry(string name)
        {
            if (ContainsEntry(name))
            {
                var entry = file.GetEntry(name);
                var m = new MemoryStream();
                using (var s = entry.Open())
                {
                    await s.CopyToAsync(m);
                    return m;
                }
            }

            return null;
        }

        public async void AddEntry(string name, Stream data)
        {
            var entry = file.CreateEntry(name);
            using (var s = entry.Open())
            {
                await data.CopyToAsync(s);
            }
        }

        public async Task<bool> AddDirectory(string directory)
        {
            var entry = file.CreateEntry(directory);
            return true;
        }

        public async Task<bool> RemoveEntry(string name)
        {
            var entry = GetEntry(name).Result;
            if (entry != null)
            {
                entry.Dispose();
                return true;
            }

            return false;
        }

        public bool ContainsEntry(string name)
        {
            foreach (var entry in file.Entries)
                if (entry.FullName == name)
                    return true;

            return false;
        }

        public List<string> GetEntries()
        {
            var names = new List<string>();
            foreach (var entry in file.Entries) names.Add(entry.FullName);
            return names;
        }

    }
}