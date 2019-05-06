using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 部门
    /// </summary>
    [Table("Sys_Department")]
    public class Department
    {
        public Department()
        {
            Children = new List<Department>();
        }

        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public double? OrderNumber { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 子菜单集
        /// </summary>
        [NotMapped]
        public IList<Department> Children { get; set; }
    }
}
