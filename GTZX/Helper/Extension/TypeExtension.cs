using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Helper.Extension
{
    public static class TypeExtension
    {
        /// <summary>
        /// 获取枚举的值/描述字典
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumDictionary(this Type type)
        {
            var dictionary = new Dictionary<int, string>();
            if (!type.IsEnum) return dictionary;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if(!field.IsLiteral) continue;
                var descriptionAttribute = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                var description = descriptionAttribute.Description ?? field.Name;
                Activator.CreateInstance(type);
                dictionary.Add(Convert.ToInt32(field.GetValue(Activator.CreateInstance(type))), description);
            }
            return dictionary;
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="fieldName">枚举字段名</param>
        /// <returns></returns>
        public static string GetEnumDescription(this Type type, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;
            var field = type.GetField(fieldName);
            if (field == null) return string.Empty;
            var description = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return description.Description ?? field.Name;
        }
    }
}
