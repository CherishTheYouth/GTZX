using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using Common;
using Helper.Extension;
using log4net;
namespace Console
{
    /// <summary>
    /// 异常处理
    /// </summary>
    public static class ExceptionHandler
    {
        public static HttpResponseMessage HandleException(Exception exception)
        {
            var message = GetExceptionMessage(exception);

            return new ServiceInvokeResult { Result = false, Message = message }.ToJsonMessage();
        }

        public static string GetExceptionMessage(Exception exception)
        {
            if (!(exception is KnownException))
            {
                var log = LogManager.GetLogger("Console");
                log.Error(exception.Message, exception);
            }
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            var newException = exception as DbEntityValidationException;
            if (newException == null) return exception.Message;

            var errorMessages = new List<string>();
            foreach (var validationResult in newException.EntityValidationErrors)
            {
                var entityName = validationResult.Entry.Entity.GetType().Name;
                errorMessages.AddRange(validationResult.ValidationErrors.Select(error => entityName + "." + error.PropertyName + ": " + error.ErrorMessage));
            }
            return errorMessages.FirstOrDefault();
        }
    }

    public class KnownException : Exception
    {
        public KnownException(string message) : base(message)
        {

        }
    }
}