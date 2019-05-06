using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace WcfContract.Wcf
{
    internal class ClientMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // 如果配置包含授权码，添加进Header
            var authCode = ConfigurationManager.AppSettings["AuthCode"];
            if (!string.IsNullOrEmpty(authCode))
            {
                request.Headers.Add(MessageHeader.CreateHeader("ac", string.Empty, authCode));
            }
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {

        }
    }
}