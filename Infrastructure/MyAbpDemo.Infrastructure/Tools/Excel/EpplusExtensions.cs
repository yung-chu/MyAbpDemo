using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace MyAbpDemo.Infrastructure
{
    /// <summary>
    /// EPPLus扩展类
    /// 使用Open Office XML（Xlsx）文件格式
    /// 能读写Excel 2007/2010文件的开源组件
    /// </summary>
    public static class EpplusExtensions
    {
        /// <summary>
        /// 最大行数
        /// </summary>
        public const int MaxRows = ExcelPackage.MaxRows;
        /// <summary>
        /// 最大列数
        /// </summary>
        public const int MaxColumns = ExcelPackage.MaxColumns;

        public static List<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet, ref StringBuilder stringBuilder) where T : new()
        {
                //对象所有列属性
                List<PropertyInfoModel> columns = new List<PropertyInfoModel>();
                var properties = typeof(T).GetProperties().ToList();
                foreach (var propertyInfo in properties)
                {
                    int column = properties.IndexOf(propertyInfo) + 1;
                    var displayNameAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayNameAttribute));
                    var columnName = displayNameAttribute == null ? string.Empty : ((DisplayNameAttribute)displayNameAttribute).DisplayName.Trim();

                    columns.Add(new PropertyInfoModel
                    {
                        Property = propertyInfo,
                        Column = column,
                        ColumnName = columnName
                    });
                }

                //Excel所有行数
               var rows = worksheet.Cells.Where(a => a.Value != null)
                    .Select(cell => cell.Start.Row)
                    .Distinct()
                    .OrderBy(x => x);

               //Excel所有列标题
               var columnNames = new List<string>();
               var cellRows = worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, 1,
                    worksheet.Dimension.End.Column];
               foreach (var firstRowCell in cellRows)
               {
                   string text = firstRowCell.Text.Trim();
                   if (text != "")
                   {
                       columnNames.Add(text);
                   }
               }

               List<T> list = new List<T>();
               var totalRows = rows.Skip(1).ToList();//去掉首行标题

               #region 数据类型检测、数据范围(错误信息以逗号分隔)
                if (!rows.Any())
                {
                    stringBuilder.Append("excel无数据,请下载模板重新上传");
                    return list;
                }
                else if (!totalRows.Any())
                {
                    stringBuilder.Append("excel无数据,请重新上传");
                    return list;
                }
                if (totalRows.First() > MaxRows)
                {
                    stringBuilder.AppendFormat("数据量过大,超过{0},请重新上传", ExcelPackage.MaxRows);
                    return list;
                }
                if (columnNames.Count > MaxColumns)
                {
                    stringBuilder.AppendFormat("列名太长,超过{0},请重新上传", ExcelPackage.MaxColumns);
                    return list;
                }
                if (!columns.Select(a => a.ColumnName).SequenceEqual(columnNames))
                {
                    stringBuilder.AppendFormat("excel标题与模板标题不一致，请下载模板重新上传");
                    return list;
                }

                foreach (var row in totalRows)
                {
                    var tnew = new T();

                    foreach (var col in columns)
                    {
                        var val = worksheet.Cells[row, col.Column];

                        if (val.Value == null)//数据为空默认空字符
                        {
                            val.Value = string.Empty;
                        }
                        if (col.Property.PropertyType == typeof(short))
                        {
                            if (!short.TryParse(val.Value.ToString(), out short a))
                            {
                                stringBuilder.Append($"第{row}行,第{col.Column}列数据格式不是整型;");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<short>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(Int32))
                        {
                            //空字符默认值0
                            if (string.IsNullOrEmpty(val.Value.ToString()))
                            {
                                val.Value = 0;
                            }

                            if (!int.TryParse(val.Value.ToString(), out int a))
                            {
                                stringBuilder.Append($"第{row}行,第{col.Column}列数据格式不是整型;");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<int>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(long))
                        {
                            if (!long.TryParse(val.Value.ToString(), out long a))
                            {
                                stringBuilder.Append($"第{row}行,第{col.Column}列数据格式不是长整型;");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<long>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(decimal))
                        {
                            if (!decimal.TryParse(val.Value.ToString(), out decimal a))
                            {
                                stringBuilder.Append($"第{row}行,第{col.Column}列数据格式不是数字型;");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<decimal>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(float))
                        {
                            if (!float.TryParse(val.Value.ToString(), out float a))
                            {
                                stringBuilder.Append($"第{row}行,第{col.Column}列数据格式不是数字型;");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<float>());
                            }
                        }
                       if (col.Property.PropertyType == typeof(DateTime))
                        {
                            if (!DateTime.TryParse(val.Value.ToString(), out DateTime a))
                            {
                                stringBuilder.Append($"第{row}行,第{col.Column}列数据格式不是日期类型;");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<DateTime>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(String))
                        {
                            if (!IsSafeSqlString(val.Value.ToString()))
                            {
                                stringBuilder.Append($"第{row}行,第{col.Column}列含有特殊字符;");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<String>());
                            }
                        }
                    }

                list.Add(tnew);
            }
            #endregion

              return list;
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
    }

    /// <summary>
    /// 用于excel列包装
    /// </summary>
    public class PropertyInfoModel
    {
        /// <summary>
        /// 当前字段属性
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// 当前列数
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
    }
}
