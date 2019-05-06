using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [Table("Sys_Log")]
    public class Log
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [MaxLength(200)]
        public string Content { get; set; }

        /// <summary>
        /// 详细变更内容（一般是json字符串）
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public LogType Type { get; set; }

        /// <summary>
        /// 关联的对象Id
        /// </summary>
        public Guid? TargetId { get; set; }

        /// <summary>
        /// 操作用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
