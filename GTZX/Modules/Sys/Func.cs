using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 功能
    /// </summary>
    [Table("Sys_Func")]
    public class Func
    {
        public Func()
        {
            Children = new List<Func>();
        }

        public Guid Id { get; set; }

        /// <summary>
        /// 父功能Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 功能代码
        /// </summary>
        [MaxLength(50)]
        public string FuncCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public double? OrderNumber { get; set; }

        /// <summary>
        /// 子功能集
        /// </summary>
        [NotMapped]
        public IList<Func> Children { get; set; }
    }
}
