using System;
using System.ServiceModel;
using Helper;

namespace WcfService
{
    public class ExceptionHandler
    {
        public static Exception HandleException(Exception exception)
        {
            if (exception is FaultException)
            {
                return exception;
            }
            LogHelper.WriteLog(exception);
            return new FaultException(exception.Message);
        }
    }
}
