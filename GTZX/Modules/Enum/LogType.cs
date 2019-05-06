using System.ComponentModel;

namespace Modules
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        Default,

        [Description("新增")]
        Add,

        [Description("修改")]
        Modify,

        [Description("删除")]
        Delete
    }
}
