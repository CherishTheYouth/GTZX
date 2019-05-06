using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules
{
    /// <summary>
    /// 登录用户表
    /// </summary>
    [Table("Sys_User")]
    public class User
    {
        public User()
        {
            IsEnable = true;
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Password { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        [NotMapped]
        public bool IsAdmin { get; set; }
    }
}
