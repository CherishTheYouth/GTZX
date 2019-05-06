using System;

namespace WcfContract
{
    /// <summary>
    /// WCF接口路由Attribute，用于标注每个interface的访问路由。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RouteAttribute : Attribute
    {
        public RouteAttribute()
        {

        }

        public RouteAttribute(string routeName)
        {
            RouteName = routeName;
        }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; }
    }
}
