using System;
using System.Linq;
using System.Web.Mvc;

namespace Console
{
    public static class ControllerExtension
    {
        /// <summary>
        /// 验证输入
        /// </summary>
        /// <param name="controller"></param>
        public static void ValidateModel(this Controller controller)
        {
            if (controller.ModelState.IsValid) return;
            var error = controller.ModelState.First(x => x.Value.Errors.Count > 0);
            throw new Exception(error.Value.Errors[0].ErrorMessage);
        }
    }
}