using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Table("Sys_Menu")]
    public class Menu
    {
        public Menu()
        {
            Children = new List<Menu>();
        }

        public Guid Id { get; set; }

        /// <summary>
        /// 父菜单Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 菜单Url
        /// </summary>
        [MaxLength(200)]
        public string Url { get; set; }

        /// <summary>
        /// 图标样式
        /// </summary>
        [MaxLength(50)]
        public string IconClass { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public double? OrderNumber { get; set; }

        /// <summary>
        /// 子菜单集
        /// </summary>
        [NotMapped]
        public IList<Menu> Children { get; set; }
    }
}
