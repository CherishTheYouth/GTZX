using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 角色菜单关联表
    /// </summary>
    [Table("Sys_RoleMenu")]
    public class RoleMenu
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Guid MenuId { get; set; }
    }
}
