using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Helper.Extension
{
    public static class StreamExtension
    {
        /// <summary>
        /// 根据流获取md5码
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMd5(this Stream stream)
        {
            var md5 = new MD5CryptoServiceProvider();
            var buffer = md5.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var t in buffer)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
