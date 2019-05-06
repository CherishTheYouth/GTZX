using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 人员标签关联表
    /// </summary>
    [Table("Sys_EmployeeTag")]
    public class EmployeeTag
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid TagId { get; set; }
    }
}
