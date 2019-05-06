using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helper;

namespace Console.Util
{
    public class ConfigUtil
    {
        /// <summary>
        /// 获取api的路径
        /// </summary>
        /// <returns></returns>
        public static string GetApiUrl()
        {
            return ConfigurationHelper.GetAppSetting("ApiUrl") ?? string.Empty;
        }

    }
}