using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Helper;

namespace WcfService.Wcf
{
    public class DispatchMessageInspector : IDispatchMessageInspector
    {
        private string[] authCodes;
        private bool? authEnable;

        /// <summary>
        /// 允许的客户端授权码，多个授权码用逗号（,）隔开
        /// </summary>
        public string[] AuthCodes
        {
            get
            {
                return authCodes ??
                       (authCodes =
                           (ConfigurationHelper.GetAppSetting("AuthCodes") ?? string.Empty).Split(',')
                               .Where(x => !string.IsNullOrEmpty(x))
                               .ToArray());
            }
        }

        /// <summary>
        /// 是否启用客户端授权
        /// </summary>
        public bool AuthEnable
        {
            get
            {
                if (authEnable.HasValue) return authEnable.Value;
                try
                {
                    authEnable = bool.Parse(ConfigurationHelper.GetAppSetting("AuthEnable") ?? "False");
                }
                catch
                {
                    authEnable = true;
                }
                return authEnable.Value;
            }
        }

        public object AfterReceiveRequest(ref Message request,
            IClientChannel channel, InstanceContext instanceContext)
        {
            if (!AuthEnable) return null;

            // 客户端授权认证
            var i = request.Headers.FindHeader("ac", string.Empty);
            if (i == -1 || !AuthCodes.Contains(request.Headers.GetHeader<string>("ac", string.Empty)))
                throw new FaultException("访问未授权");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState">AfterReceiveRequest方法的返回值</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // 请求的接口信息
            //var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
        }
    }
}
