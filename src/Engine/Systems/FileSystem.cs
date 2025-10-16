using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Red.Common;
using Red.Types;


namespace Red.Systems
{
    public class FileSystem : Singleton<FileSystem>, IDisposable
    {
        private readonly List<Package> packages = new List<Package>();

        public void Dispose()
        {
            packages.Clear();
        }

        public string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public string GetFilePath(string relativeFilename)
        {
            return Path.GetFullPath(AssemblyDirectory + relativeFilename);
        }

        public Stream GetFile(string relativeFilename)
        {
            var absolutePath = GetFilePath(relativeFilename);
            if (File.Exists(absolutePath))
            {
                return File.Open(absolutePath, FileMode.Open);
            }

            //Search the packages for the file...
            foreach (var package in packages)
                if (package.ContainsEntry(relativeFilename))
                {
                    var result = package.GetEntry(relativeFilename).Result;
                    return result;
                }

            return null;
        }

        public byte[] GetFileBytes(string relativeFilename)
        {
            var absolutePath = GetFilePath(relativeFilename);
            if (File.Exists(absolutePath))
                using (var s = File.Open(absolutePath, FileMode.Open))
                using (var reader = new BinaryReader(s))
                    return reader.ReadBytes((int)s.Length);
            return null;
        }

        public bool LoadPackage(string relativeFilename)
        {
            var fullFilename = GetFilePath(relativeFilename);
            if (File.Exists(fullFilename))
            {
                var package = new Package(fullFilename);
                packages.Add(package);
                return true;
            }

            return false;
        }

        public MemoryStream GetPackageContent(string name)
        {
            foreach (var package in packages)
                if (package.ContainsEntry(name))
                    return package.GetEntry(name).Result;
            Log.Error(string.Format("Entry {0} not found in any loaded packages"));
            return null;
        }

        internal MemoryStream SaveJson<T>(T data)
        {
            var json = JsonConvert.SerializeObject(data);
            return Task.Run(() =>
            {
                var stream = new MemoryStream();

                var d = Serialization.WriteString(json);
                stream.WriteAsync(d, 0, d.Length);
                return stream;
            }).Result;
        }

        internal T LoadJson<T>(MemoryStream stream)
        {
            return Task<T>.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(
                Serialization.ReadString(stream.ToArray()))).Result;
        }

        internal MemoryStream Save<T>(T data)
        {
            return Task.Run(() =>
            {
                var stream = new MemoryStream();
                Serialization.WriteStruct(stream, data);
                return stream;
            }).Result;

        }

        internal T Load<T>(MemoryStream stream)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                return Serialization.ReadStruct<T>(stream);
            }).Result;
        }
    }
}