using System.Web.Mvc;
using Helper;

namespace Portals
{
    public class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            LogHelper.WriteLog(exception);
        }
    }
}