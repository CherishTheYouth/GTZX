using System.Configuration;

namespace Helper
{
    /// <summary>
    /// 配置文件辅助类
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// 获取指定key对应的AppSetting值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 获取指定键值的数据库连接设置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ConnectionStringSettings GetConnectionSetting(string key)
        {
            return ConfigurationManager.ConnectionStrings[key];
        }

        /// <summary>
        /// 设置对应Key的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static void SetAppSetting(string key, string value)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[key] == null)
            {
                configuration.AppSettings.Settings.Add(key, value);
            }
            else
            {
                configuration.AppSettings.Settings[key].Value = value;
            }
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
