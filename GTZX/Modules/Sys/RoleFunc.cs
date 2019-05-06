using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 角色功能关系表
    /// </summary>
    [Table("Sys_RoleFunc")]
    public class RoleFunc
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public Guid FuncId { get; set; }
    }
}
