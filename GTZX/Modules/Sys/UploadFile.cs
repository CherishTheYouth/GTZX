using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 上传文件
    /// </summary>
    [Table("Sys_UploadFile")]
    public class UploadFile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Path { get; set; }

        /// <summary>
        /// 缩略图/快照存放路径
        /// </summary>
        [MaxLength(200)]
        public string ShortcutPath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Range(0, long.MaxValue)]
        public long FileSize { get; set; }

        public DateTime UploadTime { get; set; }

        public Guid? UploadUserId { get; set; }
    }
}
