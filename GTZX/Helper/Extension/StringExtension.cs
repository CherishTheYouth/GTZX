using System.Text.RegularExpressions;

namespace Helper.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// 是否是手机号
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public static bool IsMobile(this string mobileNumber)
        {
            return Regex.IsMatch(mobileNumber, @"^1(3[0-9]|4[57]|5[0-35-9]|7[0135678]|8[0-9])\d{8}$");
        }

        /// <summary>
        /// 是否是邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(this string email)
        {
            return Regex.IsMatch(email,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 验证密码是否合法（由8-16位字母、数字或下划线组成）
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsPassword(this string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-Z][a-zA-Z0-9_]{7,15}$");
        }

        /// <summary>
        /// 是否是姓名
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsName(this string text)
        {
            return Regex.IsMatch(text, @"^[\u4e00-\u9fa5]$");
        }

        /// <summary>
        /// 判断是否是身份证
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsIdentityCard(this string text)
        {
            return Regex.IsMatch(text, @"^(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$)|(^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[Xx])$)$");
        }

        /// <summary>
        /// 是否是图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsImage(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var index = fileName.LastIndexOf('.');
            if (index < 0) return false;
            var extension = fileName.Substring(index).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".png":
                case ".bmp":
                case ".jpeg":
                case ".gif":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 是否是版本号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsVersion(this string text)
        {
            return Regex.IsMatch(text, @"^\d{1,4}(\.\d{1,10}){2,3}$");
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetVersion(this string text)
        {
            var regex = new Regex(@"\d{1,4}(\.\d{1,10}){2,3}");
            foreach(Match m in regex.Matches(text))
            {
                return m.Value;
            }
            return string.Empty;
        }
    }
}
