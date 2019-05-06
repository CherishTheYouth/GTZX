using System.Web.Http.Filters;
using Common;
using Helper.Extension;

namespace Console
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var message = ExceptionHandler.GetExceptionMessage(exception);
            actionExecutedContext.Response = new ServiceInvokeResult { Result = false, Message = message }.ToJsonMessage();

            base.OnException(actionExecutedContext);
        }
    }
}