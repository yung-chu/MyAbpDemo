using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MyAbpDemo.Infrastructure
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取<see cref="Nullable{TValue}"/>范型的构造类型
        /// </summary>
        public static Type GetTypeOfNullable(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : null;
        }

        /// <summary>
        /// 判断类型是否为 可空类型 <see cref="Nullable{TValue}"/>
        /// </summary>
        public static bool IsNullableType(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// 获取枚举指定的显示内容
        /// </summary>
        public static object Display(this MemberInfo memberInfo, DisplayProperty property)
        {
            if (memberInfo == null) return null;

            var display = memberInfo.GetCustomAttribute<DisplayAttribute>();

            if (display != null)
            {
                switch (property)
                {
                    case DisplayProperty.Name:
                        return display.GetName();
                    case DisplayProperty.ShortName:
                        return display.GetShortName();
                    case DisplayProperty.GroupName:
                        return display.GetGroupName();
                    case DisplayProperty.Description:
                        return display.GetDescription();
                    case DisplayProperty.Order:
                        return display.GetOrder();
                    case DisplayProperty.Prompt:
                        return display.GetPrompt();
                }
            }

            return null;
        }

        /// <summary>
        /// 获取枚举说明DisplayName
        /// </summary>
        public static string DisplayName(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Name) as string;
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        public static string DisplayShortName(this MemberInfo val)
        {
            return val.Display(DisplayProperty.ShortName) as string;
        }

        /// <summary>
        /// 获取枚举分组名称
        /// </summary>
        public static string DisplayGroupName(this MemberInfo val)
        {
            return val.Display(DisplayProperty.GroupName) as string;
        }

        /// <summary>
        /// 获取枚举水印信息
        /// </summary>
        public static string DisplayPrompt(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Prompt) as string;
        }

        /// <summary>
        /// 获取枚举备注
        /// </summary>
        public static string DisplayDescription(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Description) as string;
        }

        /// <summary>
        /// 获取枚举显示排序
        /// </summary>
        public static int? DisplayOrder(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Order) as int?;
        }
    }

    /// <summary>
    /// 定义 <see cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/> 的属性
    /// </summary>
    public enum DisplayProperty
    {
        /// <summary>
        /// 名称
        /// </summary>
        Name,

        /// <summary>
        /// 短名称
        /// </summary>
        ShortName,

        /// <summary>
        /// 分组名称
        /// </summary>
        GroupName,

        /// <summary>
        /// 说明
        /// </summary>
        Description,

        /// <summary>
        /// 排序
        /// </summary>
        Order,

        /// <summary>
        /// 水印信息
        /// </summary>
        Prompt,
    }
}