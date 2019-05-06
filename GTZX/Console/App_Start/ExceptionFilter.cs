using System.Web.Mvc;
using log4net;

namespace Console
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
            var log = LogManager.GetLogger("Console");
            log.Error(exception);
        }
    }
}