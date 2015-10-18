using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files
{
    public static class Files
    {
        public static bool IsExist(string path)
        {
            if (!File.Exists(path))
                return false;

            return true;
        }

        public static void OpenRw(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open);
        }

        public static void OpenReadOnly(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public static void OpenWriteOnly(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write);
        }

        public static void OpenWriting(string path)
        {
            var fileStream = new FileStream(path, FileMode.Append);
        }

        public static void CreateFileRw(string path)
        {
            var fileStream = new FileStream(path, FileMode.CreateNew);
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static IList<String> ReadFileToList(string path)
        {
            var list = new List<string>();
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line); // Add to list.
                }
            }

            return list;
        }

        public static IEnumerable<T> ReadFile<T>(string path, Func<string, T> makeReader)
        {
            using (var sr = new StreamReader(path))
            {
                var readLine = sr.ReadLine();
                while (!string.IsNullOrEmpty(readLine))
                {
                    yield return makeReader(readLine);
                    readLine = sr.ReadLine();
                }
            }
        }
    }
}
