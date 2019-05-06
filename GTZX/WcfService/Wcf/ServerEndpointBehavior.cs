using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WcfService.Wcf
{
    public class ServerEndpointBehavior : BehaviorExtensionElement, IEndpointBehavior
    {

        public override Type BehaviorType
        {
            get { return typeof (ServerEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new ServerEndpointBehavior();
        }

        public void AddBindingParameters(ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint,
            EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new DispatchMessageInspector());
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
}
