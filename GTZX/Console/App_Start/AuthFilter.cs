using System.Web.Mvc;

namespace Console
{
    /// <summary>
    /// 用于登录授权验证的筛选器。
    /// </summary>
    public class AuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 如果用户未登录，且action未明确标识可跳过登录授权，则跳转到登录页面
            if (!CacheUtil.IsLogin && !filterContext.ActionDescriptor.IsDefined(typeof(AuthEscape), false))
            {
                const string loginUrl = "~/Main/Login";
                filterContext.Result = new RedirectResult(loginUrl);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}