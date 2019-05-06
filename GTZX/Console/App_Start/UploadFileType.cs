using Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Console
{
    public class UploadFileType
    {
        public static string TypeString(int num = 0)
        {
            string groupStr = "";
            switch (num)
            {
                case 0:
                    groupStr = UploadFileTypeEnum.Default.ToString();
                    break;
                case 1:
                    groupStr = UploadFileTypeEnum.Project_Type1.ToString();
                    break;
                default:
                    break;
            }
            return groupStr;
        }
    }
}