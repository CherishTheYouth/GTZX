using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 服务调用结果类
    /// </summary>
    public class ServiceInvokeResult
    {

        public ServiceInvokeResult()
        {
            Result = true;
        }

        public ServiceInvokeResult(object data)
        {
            Data = data;
            Result = true;
        }

        /// <summary>
        /// 服务调用结果
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

    }
}
