using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web.Script.Serialization;
using System.Net.Http;

namespace Helper.Extension
{
    public static class ObjExtension
    {
        /// <summary>
        /// 将对象实例变更为指定类型的实例
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转化后的对象实例</returns>
        public static object ChangeType(this object obj, Type targetType)
        {
            // 类型转化的前提是其实现了IConvertible接口
            if (!(obj is IConvertible))
            {
                return null;
            }
            // 可空类型的转化
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (obj.ToString().Length == 0) return null;

                var nullableConverter = new NullableConverter(targetType);
                targetType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(obj, targetType);
        }

        /// <summary>
        /// 根据字典提供的值设置对象属性的值
        /// </summary>
        /// <param name="obj">待设置值的对象</param>
        /// <param name="valueDictionary">包含属性键值的字典</param>
        public static void SetValues(this object obj, Dictionary<string, object> valueDictionary)
        {
            var type = obj.GetType();
            foreach (var item in valueDictionary)
            {
                var propertyInfo = type.GetProperty(item.Key);
                if (propertyInfo == null || !(item.Value is IConvertible)) continue;
                propertyInfo.SetValue(obj, item.Value.ChangeType(propertyInfo.PropertyType), null);
            }
        }

        /// <summary>
        /// 根据源对象设置目标对象指定属性的值
        /// </summary>
        /// <param name="propertyNameCollection">字段列表</param>
        /// <param name="targetObj">目标对象</param>
        /// <param name="sourceObj">源对象</param>
        public static void SetValues(this object targetObj, object sourceObj, ICollection<string> propertyNameCollection)
        {
            var targetType = targetObj.GetType();
            var sourceType = sourceObj.GetType();
            foreach (var propertyName in propertyNameCollection)
            {
                var propertyOfTarget = targetType.GetProperty(propertyName);
                var propertyOfSource = sourceType.GetProperty(propertyName);
                // 以下情况不设置属性的值：
                // 1、源对象或目标对象没有该属性；
                // 2、源对象和目标对象对应的属性类型不同；
                // 3、源对象对应的属性不可写。
                if (propertyOfTarget == null || propertyOfSource == null || propertyOfTarget.PropertyType != propertyOfSource.PropertyType ||
                    !propertyOfTarget.CanWrite) continue;
                propertyOfTarget.SetValue(targetObj, propertyOfSource.GetValue(sourceObj));
            }
        }

        /// <summary>
        /// 比较两个对象指定的属性值，返回值不相等的属性字典集合
        /// </summary>
        /// <param name="targetObj">变更后的对象</param>
        /// <param name="sourceObj">原对象</param>
        /// <param name="propertyNameCollection">要比较的属性集合</param>
        /// <returns></returns>
        public static ICollection<ObjectChangeInfo> CompareDifference(this object targetObj, object sourceObj,
            ICollection<string> propertyNameCollection)
        {
            var list = new Collection<ObjectChangeInfo>();
            propertyNameCollection = (propertyNameCollection ?? new Collection<string>()).Distinct().ToList();

            var targetType = targetObj.GetType();
            var sourceType = sourceObj.GetType();
            foreach (var propertyName in propertyNameCollection)
            {
                var propertyOfTarget = targetType.GetProperty(propertyName);
                var propertyOfSource = sourceType.GetProperty(propertyName);
                var valueOfTarget = (propertyOfTarget == null) ? null : propertyOfTarget.GetValue(targetObj);
                var valueOfSource = (propertyOfSource == null) ? null : propertyOfSource.GetValue(sourceObj);
                if (valueOfTarget == null)
                {
                    if (valueOfSource == null) continue;
                    list.Add(new ObjectChangeInfo
                    {
                        Field = propertyName,
                        OValue = valueOfSource,
                        CValue = DBNull.Value
                    });
                }
                else
                {
                    if (!valueOfTarget.Equals(valueOfSource))
                    {
                        list.Add(new ObjectChangeInfo
                        {
                            Field = propertyName,
                            OValue = valueOfSource,
                            CValue = valueOfTarget
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 比较两个相同类型对象的所有属性，返回值不相等的属性字典集合
        /// </summary>
        /// <param name="targetObj">变更后的对象</param>
        /// <param name="sourceObj">原对象</param>
        /// <returns></returns>
        public static ICollection<ObjectChangeInfo> CompareDifference(this object targetObj, object sourceObj)
        {
            var list = new Collection<ObjectChangeInfo>();
            var targetType = targetObj.GetType();
            var sourceType = sourceObj.GetType();
            if (!(targetType == sourceType)) throw new Exception("要比较的对象类型不一致。");
            foreach (var property in targetType.GetProperties())
            {
                var valueOfTarget = property.GetValue(targetObj);
                var valueOfSource = property.GetValue(sourceObj);
                if (valueOfTarget == null)
                {
                    if (valueOfSource == null) continue;
                    list.Add(new ObjectChangeInfo
                    {
                        Field = property.Name,
                        OValue = valueOfSource,
                        CValue = DBNull.Value
                    });
                }
                else
                {
                    if (!valueOfTarget.Equals(valueOfSource))
                    {
                        list.Add(new ObjectChangeInfo
                        {
                            Field = property.Name,
                            OValue = valueOfSource,
                            CValue = valueOfTarget
                        });
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 将对象转换为
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage ToJsonMessage(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            var text = serializer.Serialize(obj);
            var result = new HttpResponseMessage { Content = new StringContent(text, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        public static HttpResponseMessage ToDateFormatedJsonMessage(this object obj, string dateTimeFormat = "yyyy/MM/dd HH:mm:ss")
        {
            //var serializer = new JavaScriptSerializer();
            var text = JsonConvert.SerializeObject(obj, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = dateTimeFormat });
            //var text = serializer.Serialize(obj,Formatting.Indented,new IsoDateTimeConverter {DateTimeFormat=dateTimeFormat });
            var result = new HttpResponseMessage { Content = new StringContent(text, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }



    }

    /// <summary>
    /// 对象变化信息
    /// </summary>
    public class ObjectChangeInfo
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 原始值
        /// </summary>
        public dynamic OValue { get; set; }

        /// <summary>
        /// 现有值
        /// </summary>
        public dynamic CValue { get; set; }
    }
}
