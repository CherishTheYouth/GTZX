using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 人员表
    /// </summary>
    [Table("Sys_Employee")]
    public class Employee
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string FullName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public bool Gender { get; set; }

        /// <summary>
        /// 出生年月
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(50)]
        public string IdCard { get; set; }

        // 是否删除
        public bool IsDelete { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { set; get; }
    }
}
