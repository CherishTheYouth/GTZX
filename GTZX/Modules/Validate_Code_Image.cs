using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules
{
    /// <summary>
    /// 验证码图片
    /// </summary>
    [Table("VALIDATE_CODE_IMAGE")]
    public class Validate_Code_Image
    {
        [Key]
        public string IDS { get; set; }

        /// <summary>
        /// 验证码算出值
        /// </summary>
        public string VALIDATEVALUE { get; set; }

        /// <summary>
        /// 验证码图片
        /// </summary>
        public byte[] VALIDATEIMAGE { get; set; }
    }
}
