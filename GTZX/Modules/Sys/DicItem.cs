using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 字典项
    /// </summary>
    [Table("Sys_DicItem")]
    public class DicItem
    {
        public DicItem()
        {
            Dic = Dic.Default;
            IsDelete = false;
            Children = new List<DicItem>();
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 父项Id
        /// </summary>
        public Guid? ParentId { get; set; }

        public Dic Dic { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Remark { get; set; }

        public double? OrderNumber { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreateTime { get; set; }

        [NotMapped]
        public IList<DicItem> Children { get; set; }
    }

        public class GroupAttribute : Attribute
        {
            public string Text { get; set; }
        }
}
