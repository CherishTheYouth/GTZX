using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Console
{
    /// <summary>
    /// Object扩展类
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 获取HttpResponseMessage
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetJsonMessage(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            var text = serializer.Serialize(obj);
            var result = new HttpResponseMessage { Content = new StringContent(text, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        /// <summary>
        /// 获取Json结果
        /// </summary>
        /// <returns></returns>
        public static JsonResult GetJsonResult(this object obj)
        {
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = obj
            };
        }

        /// <summary>
        /// 获取日期序列化后的json结果
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static JsonResult GetDateFormattedJsonResult(this object obj, string dateFormat = "yyyy/MM/dd HH:mm:ss")
        {
            return new DateFormatedJson
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = obj,
                DateTimeFormat = dateFormat
            };
        }
    }
}