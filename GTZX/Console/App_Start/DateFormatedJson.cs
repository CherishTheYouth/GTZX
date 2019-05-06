using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Console
{
    public class DateFormatedJson : JsonResult
    {
        public DateFormatedJson()
        {
            DateTimeFormat = "yyyy/MM/dd HH:mm:ss";
        }

        /// <summary>
        /// 时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (Data != null)
            {
                context.HttpContext.Response.Write(JsonConvert.SerializeObject(Data, Formatting.Indented,
                    new IsoDateTimeConverter { DateTimeFormat = DateTimeFormat }));
            }
        }
    }
}