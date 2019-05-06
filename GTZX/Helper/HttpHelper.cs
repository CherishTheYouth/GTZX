using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Helper
{
    public class HttpHelper
    {
        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreateGetHttpResponse(string url, IDictionary<string, string> headers = null, CookieCollection cookies = null)
        {
            HttpWebRequest request;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.Method = "GET";

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key,header.Value);
                }
            }

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, IDictionary<string, string> headers = null, CookieCollection cookies = null)
        {
            HttpWebRequest request;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            if (parameters != null && parameters.Count > 0)
            {
                var sb = new StringBuilder();
                var i = 0;
                foreach (var key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        sb.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(parameters[key]));
                    }
                    else
                    {
                        sb.AppendFormat("{0}={1}", key, HttpUtility.UrlEncode(parameters[key]));
                        i++;
                    }
                }
                var data = Encoding.ASCII.GetBytes(sb.ToString());
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (var stream = webresponse.GetResponseStream())
            {
                if (stream == null) return string.Empty;
                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return errors == SslPolicyErrors.None;
        }
    }
}
