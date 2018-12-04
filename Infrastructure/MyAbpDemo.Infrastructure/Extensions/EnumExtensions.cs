using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MyAbpDemo.Infrastructure
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举值定义的 DisplayName
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string DisplayName(this Enum value)
        {
            var field = GetEnumField(value);

            var displayName = field.DisplayName();
            if (!string.IsNullOrEmpty(displayName))
            {
                return displayName;
            }

            var attribute = field.GetCustomAttribute<DisplayNameAttribute>();
            return attribute?.DisplayName ?? value.ToString();
        }

        /// <summary>
        /// 获取枚举值定义的 Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Description(this Enum value)
        {
            var field = GetEnumField(value);
            var description = field.DisplayDescription();

            return description ?? "";
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        private static MemberInfo GetEnumField(Enum value)
        {
            var enumType = value.GetType();
            var name = value.ToString();
            return  enumType.GetField(name);
        }

        /// <summary>
        /// 获取枚举信息
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<EnumItemInfo> GetEnumInfoList(this Enum enumType)
        {
            List <EnumItemInfo> list=new List<EnumItemInfo>();
            foreach (var value in Enum.GetValues(enumType.GetType()))
            {
                object[] objAttrs = value.GetType().GetField(value.ToString())
                    .GetCustomAttributes(typeof(DisplayNameAttribute), true);

                if (objAttrs.Length > 0)
                {
                    var displayName =objAttrs[0] is DisplayNameAttribute displayNameAttr ? displayNameAttr.DisplayName : string.Empty;

                    list.Add(new EnumItemInfo
                    {
                        Value = Convert.ToInt32(value),
                        Text = value.ToString(),
                        DisplayName = displayName
                    });
                }
            }

            return list;
        }

        public class  EnumItemInfo
        {
            /// <summary>
            /// 枚举值
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// 枚举文本
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// 枚举描述
            /// </summary>
            public string DisplayName { get; set; }
        }
    }
}
