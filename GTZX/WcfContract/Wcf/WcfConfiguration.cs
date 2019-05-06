using System.Resources;

namespace WcfContract.Wcf
{
    /// <summary>
    /// WCF服务配置项.
    /// </summary>
    public class WcfConfiguration
    {
        private const string WcfSchemaKey = "WcfSchema";
        private const string WcfPortKey = "WcfPort";
        private const string ForceHttpEnableKey = "ForceHttpEnable";
        private const string HttpPortKey = "HttpPort";

        private readonly ResourceManager resourceManager = new ResourceManager(typeof(WcfConfigurationResource));

        /// <summary>
        /// 协议，如tcp.ip/http等
        /// </summary>
        public string WcfSchema
        {
            get { return GetValue(WcfSchemaKey); }
        }

        /// <summary>
        /// 端口
        /// </summary>
        public string WcfPort
        {
            get
            {
                return GetValue(WcfPortKey);
            }
        }

        /// <summary>
        /// 强制启用Http协议
        /// </summary>
        public bool ForceHttpEnable
        {
            get
            {
                bool forceHttpEnable;
                bool.TryParse(GetValue(ForceHttpEnableKey), out forceHttpEnable);
                return forceHttpEnable;
            }
        }

        /// <summary>
        /// Http协议端口（仅在ForceHttpEnable为True且WcfSchema不是http时生效）
        /// </summary>
        public string HttpPort
        {
            get
            {
                return GetValue(HttpPortKey);
            }
        }

        /// <summary>
        /// 根据键获取对应的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return resourceManager.GetString(key) ?? string.Empty;
        }
    }
}