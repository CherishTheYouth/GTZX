using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Biz
{

    [Table("Biz_RegulationSourceFile")]
    /// <summary>
    /// 政策法规原始文件表
    /// </summary>
    public class RegulationSourceFile
    {

        public RegulationSourceFile(){
            IsEnable = true;
        }

        //主键
        [Key]
        public Guid Id { get; set; }

        //文件名
        [Required]
        [MaxLength(50)]
        public string FileName { get; set; }

        /// <summary>
        /// 文件编号
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FileNo { get; set; }


    //    [Required]
        [MaxLength(100)]
        public string SourceFile { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 上传日期
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// 归属父类（政策文件目录）
        /// </summary>
        public Guid? ParentId { get; set; }

    }
}
