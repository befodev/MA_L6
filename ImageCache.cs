using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MA_L6
{
    internal static class ImageCache
    {
        private static Dictionary<string, string> _cache = [];
        private static string? _dir = null;


        public static void InitialiseStorage()
        {
            _dir = FileSystem.Current.CacheDirectory;
            string json = Preferences.Default.Get("cache", string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                var temp = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (temp is not null) _cache = temp; 
            }
        }

        public static bool CheckImageInCache(string url)
        {
            return _cache.ContainsKey(url);
        }

        public static ImageSource GetImageFromCache(string url)
        {
            string path = _dir + "/" + _cache[url];
            return ImageSource.FromStream(() => new MemoryStream(File.ReadAllBytes(path)));
        }

        public static void SaveImageToCache(string url, Stream bytes)
        {
            byte[] hashbytes = MD5.HashData(bytes);
            string name = Convert.ToHexString(hashbytes);


            string path = _dir + "/" + name;

            bytes.Seek(0, SeekOrigin.Begin);
            FileStream fs = File.Create(path);
            bytes.CopyTo(fs);
            fs.Close();
            bytes.Seek(0, SeekOrigin.Begin);

            _cache[url] = name;
            string output = JsonSerializer.Serialize(_cache);
            Preferences.Default.Set("cache", output);
        }

        public static void SaveJsonToCache(string url, string json)
        {
            byte[] hashbytes = MD5.HashData(Encoding.UTF8.GetBytes(json));
            string name = Convert.ToHexString(hashbytes);

            string path = _dir + "/" + name + ".json";
            File.WriteAllText(path, json);
            _cache[url] = name;
            string output = JsonSerializer.Serialize(_cache);
            Preferences.Default.Set("cache", output);
        }

        public static string GetJsonFromCache(string url)
        {
            string path = _dir + "/" + _cache[url] + ".json";
            return File.ReadAllText(path);
        }
    }
}
