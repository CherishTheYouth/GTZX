using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace WcfContract.Wcf
{
    /// <summary>
    /// WCF服务代理类，用于创建各类WCF实现的接口实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceProxy<T>
    {
        private readonly WcfConfiguration wcfConfiguration = new WcfConfiguration();

        public ServiceProxy()
        {
            ForceHttp = false;
        }

        /// <summary>
        /// 获取WCF服务地址
        /// </summary>
        public virtual string WcfUrl
        {
            get
            {
                var type = typeof (T);
                var routeAttribute = Attribute.GetCustomAttribute(type, typeof (RouteAttribute)) as RouteAttribute;
                var routeName = routeAttribute != null && !string.IsNullOrEmpty(routeAttribute.RouteName)
                    ? routeAttribute.RouteName
                    : type.Name;
                return string.Format("{0}://{1}:{2}/{3}", Schema, ServiceIp, Port, routeName);
            }
        }

        /// <summary>
        /// 获取WCF服务IP
        /// </summary>
        public virtual string ServiceIp
        {
            get
            {
                var serviceIp = ConfigurationManager.AppSettings["ServiceIp"];
                if (string.IsNullOrEmpty(serviceIp))
                    throw new Exception("接口配置缺失");
                return serviceIp;
            }
        }

        /// <summary>
        /// 获取WCF服务采用的传输协议
        /// </summary>
        public virtual string Schema
        {
            get
            {
                return ForceHttp ? "http" : wcfConfiguration.WcfSchema;
            }
        }

        /// <summary>
        /// 获取WCF服务端口
        /// </summary>
        public virtual string Port
        {
            get
            {
                var port = wcfConfiguration.WcfPort;
                if (ForceHttp && !wcfConfiguration.WcfSchema.ToLower().Equals("http"))
                {
                    port = wcfConfiguration.HttpPort;
                }
                return port;
            }
        }

        /// <summary>
        /// 获取或设置是否强制使用http协议调用接口，可通过<c>WcfConfiguration</c>类的ForceHttpEnable属性查看服务端是否强制启用了http协议支持
        /// </summary>
        public bool ForceHttp { get; set; }

        /// <summary>
        /// 获取服务的Binding信息
        /// </summary>
        public virtual Binding Binding
        {
            get
            {
                Binding binding = null;
                switch (Schema.ToLower())
                {
                    case "net.tcp":
                        binding = new NetTcpBinding(SecurityMode.None)
                        {
                            ReaderQuotas =
                                new XmlDictionaryReaderQuotas
                                {
                                    MaxStringContentLength = 2147483647,
                                    MaxArrayLength = 2147483647
                                },
                            MaxBufferPoolSize = 2147483647,
                            MaxBufferSize = 2147483647,
                            MaxReceivedMessageSize = 2147483647,
                            TransferMode = TransferMode.Buffered
                        };
                        break;
                    case "http":
                        binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
                        {
                            MaxBufferPoolSize = 2147483647,
                            MaxBufferSize = 2147483647,
                            MaxReceivedMessageSize = 2147483647,
                            TransferMode = TransferMode.Buffered
                        };
                        break;
                    case "net.pipe":
                        binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                        {
                            MaxBufferPoolSize = 2147483647,
                            MaxBufferSize = 2147483647,
                            MaxReceivedMessageSize = 2147483647,
                            TransferMode = TransferMode.Buffered
                        };
                        break;
                    case "net.msmq":
                        binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
                        {
                            MaxBufferPoolSize = 2147483647,
                            MaxReceivedMessageSize = 2147483647
                        };
                        break;
                }
                return binding;
            }
        }

        /// <summary>
        /// 创建代理，返回接口实例
        /// </summary>
        /// <returns>接口实例</returns>
        public T CreateProxy()
        {
            var serviceEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(T)), Binding,
                    new EndpointAddress(WcfUrl));
            serviceEndpoint.Behaviors.Add(new ClientEndpointBehavior());
            var serviceFactory = new ChannelFactory<T>(serviceEndpoint);
            return serviceFactory.CreateChannel();
        }
    }
}