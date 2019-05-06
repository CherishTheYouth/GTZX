using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 上传文件关联表
    /// </summary>
    [Table("Sys_UploadFileRelation")]
    public class UploadFileRelation
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 上传文件Id
        /// </summary>
        public Guid UploadFileId { get; set; }

        /// <summary>
        /// 目标对象Id
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public string Group { get; set; }
    }

    public enum UploadFileTypeEnum
    {
        [Description("默认")]
        Default = 0,

        [Group(Text = "政策文件管理-政策文件原始文件")]
        [Description("上传文档")]
        Project_Type1 = 1


    }
}
