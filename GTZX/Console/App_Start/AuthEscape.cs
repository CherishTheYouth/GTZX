using System.Web.Mvc;

namespace Console
{
    /// <summary>
    /// 用于标记无需登录授权验证的Action，无任何实现。
    /// </summary>
    public class AuthEscape : ActionFilterAttribute
    {
    }
}