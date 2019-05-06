using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Xml;
using Helper;
using WcfContract;
using WcfContract.Wcf;
using WcfService.Wcf;

namespace WcfInstaller
{
    partial class MainService : ServiceBase
    {
        private readonly ICollection<ServiceHost> serviceHosts = new Collection<ServiceHost>();
        private readonly WcfConfiguration wcfConfiguration = new WcfConfiguration();

        public MainService()
        {
            InitializeComponent();

            try
            {
                // 反射获取WcfService类库提供的所有实现类
                var types =
                    Assembly.Load("WcfService")
                        .GetTypes()
                        .Where(
                            x =>
                                x.IsClass && x.IsPublic &&
                                x.GetInterfaces()
                                    .Any(y => y.GetCustomAttributes(typeof(ServiceContractAttribute)).Any()));
                foreach (var type in types)
                {
                    AddServiceHost(type);
                }
            }
            catch (Exception exception)
            {
                LogHelper.WriteLog(exception);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                foreach (var sh in serviceHosts)
                {
                    foreach (var t in sh.Description.Endpoints)
                    {
                        var tcpBinding = t.Binding as NetTcpBinding;
                        var httpBinding = t.Binding as BasicHttpBinding;
                        var namedPipeBinding = t.Binding as NetNamedPipeBinding;
                        var msmqBinding = t.Binding as NetMsmqBinding;
                        if (tcpBinding != null)
                        {
                            tcpBinding.Security.Mode = SecurityMode.None;
                            tcpBinding.MaxBufferSize = 2147483647;
                            tcpBinding.MaxReceivedMessageSize = 2147483647;
                            tcpBinding.MaxBufferPoolSize = 2147483647;
                            tcpBinding.TransferMode = TransferMode.Buffered;
                            tcpBinding.ReaderQuotas = new XmlDictionaryReaderQuotas()
                            {
                                MaxStringContentLength = 2147483647,
                                MaxArrayLength = 2147483647
                            };
                        }
                        else if (httpBinding != null)
                        {
                            httpBinding.Security.Mode = BasicHttpSecurityMode.None;
                            httpBinding.MaxBufferPoolSize = 2147483647;
                            httpBinding.MaxBufferSize = 2147483647;
                            httpBinding.MaxReceivedMessageSize = 2147483647;
                        }
                        else if (namedPipeBinding != null)
                        {
                            namedPipeBinding.Security.Mode = NetNamedPipeSecurityMode.None;
                            namedPipeBinding.MaxBufferPoolSize = 2147483647;
                            namedPipeBinding.MaxBufferSize = 2147483647;
                            namedPipeBinding.MaxReceivedMessageSize = 2147483647;
                        }
                        else if (msmqBinding != null)
                        {
                            msmqBinding.Security.Mode = NetMsmqSecurityMode.None;
                            msmqBinding.MaxBufferPoolSize = 2147483647;
                            msmqBinding.MaxReceivedMessageSize = 2147483647;
                        }
                        t.Behaviors.Add(new ServerEndpointBehavior());
                    }
                    sh.Open();
                }
            }
            catch (Exception exception)
            {
                LogHelper.WriteLog(exception);
            }
        }

        protected override void OnStop()
        {
            foreach (var sh in serviceHosts)
            {
                StopService(sh);
            }
        }

        private void AddServiceHost(Type type)
        {
            var interfaces =
                type.GetInterfaces().Where(x => x.GetCustomAttributes(typeof(ServiceContractAttribute)).Any());
            foreach (var i in interfaces)
            {
                var routeAttribute = Attribute.GetCustomAttribute(i, typeof(RouteAttribute)) as RouteAttribute;
                var routeName = routeAttribute != null && !string.IsNullOrEmpty(routeAttribute.RouteName)
                    ? routeAttribute.RouteName
                    : i.Name;
                var url = string.Format("{0}://localhost:{1}/{2}", wcfConfiguration.WcfSchema, wcfConfiguration.WcfPort, routeName);
                var serviceHost = new ServiceHost(type, new Uri(url));
                serviceHost.AddDefaultEndpoints();
                serviceHosts.Add(serviceHost);

                // 如果强制启用http且默认配置的协议不是http，追加http协议的服务
                if (wcfConfiguration.ForceHttpEnable && !wcfConfiguration.WcfSchema.ToLower().Equals("http"))
                {
                    var httpUrl = string.Format("http://localhost:{0}/{1}", wcfConfiguration.HttpPort, routeName);
                    var httpServiceHost = new ServiceHost(type, new Uri(httpUrl));
                    httpServiceHost.AddDefaultEndpoints();
                    serviceHosts.Add(httpServiceHost);
                }
            }
        }

        private void StopService(ServiceHost host)
        {
            if (host.State != CommunicationState.Closed || host.State != CommunicationState.Closing)
            {
                host.Close();
            }
        }
    }
}
