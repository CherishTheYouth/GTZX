using System.Web.Http;

namespace Console
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 启用Web API特性路由
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            // webapi 错误处理
            config.Filters.Add(new ApiExceptionFilter());
        }
    }
}
