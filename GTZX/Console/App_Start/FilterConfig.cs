using System.Web.Mvc;

namespace Console
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // 错误处理
            filters.Add(new ExceptionFilter());

            // 登录授权
            filters.Add(new AuthFilter());
        }
    }
}