using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 政策法规表
    /// </summary>
    [Table("Biz_Regulation")]
    public class Regulation
    {
        public Regulation()
        {
            IsEnable = true;
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RegulationName { get; set; }

        /// <summary>
        /// 文件编号
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string RegulationNo { get; set; }

        /// <summary>
        /// 文件内容
        /// </summary>
        [Required]
        [MaxLength(5000)]
        public string RegulationContent { get; set; }

        /// <summary>
        /// 颁布单位
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PublishDep { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 颁布日期
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// 归属父类
        /// </summary>
        public Guid? ParentId { get; set; }

    }
}
