using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 标签
    /// </summary>
    [Table("Sys_Tag")]
    public class Tag
    {
        public Tag()
        {
            Type = TagType.Default;
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public double? OrderNumber { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public TagType Type { get; set; }
    }
}
