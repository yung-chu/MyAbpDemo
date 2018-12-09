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

        public static List<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet, List<ValidatorErrorInfo> errorInfo) where T : new()
        {
              #region 准备条件
               //对象所有列属性
               List<PropertyInfoModel> columnPropertyInfos = new List<PropertyInfoModel>();
                var properties = typeof(T).GetProperties().ToList();
                foreach (var propertyInfo in properties)
                {
                    int column = properties.IndexOf(propertyInfo) + 1;
                    var displayNameAttribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayNameAttribute));
                    var columnName = displayNameAttribute == null ? string.Empty : ((DisplayNameAttribute)displayNameAttribute).DisplayName.Trim();

                    columnPropertyInfos.Add(new PropertyInfoModel
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

               foreach (var cellRow in cellRows)
               {
                   string text = cellRow.Text.Trim();
                   if (text != "")
                   {
                       columnNames.Add(text);
                   }
               }
            #endregion

              #region 数据类型检测、数据范围
                 List<T> list = new List<T>();
                 var totalRows = rows.Skip(1).ToList();//去掉首行标题

                if (!rows.Any())
                {
                    GetExcelDetailError(errorInfo, "excel无数据,请下载模板重新上传");
                    return list;
                }
                else if (!totalRows.Any())
                {
                    GetExcelDetailError(errorInfo, "excel无数据,请重新上传");
                    return list;
                }
                if (totalRows.First() > MaxRows)
                {
                    GetExcelDetailError(errorInfo, $"数据量过大,超过{ExcelPackage.MaxRows},请重新上传");
                    return list;
                }
                if (columnNames.Count > MaxColumns)
                {
                    GetExcelDetailError(errorInfo, $"列名太长,超过{ExcelPackage.MaxColumns},请重新上传");
                    return list;
                }
                if (!columnPropertyInfos.Select(a => a.ColumnName).SequenceEqual(columnNames))
                {
                    GetExcelDetailError(errorInfo, "excel标题与模板标题不一致，请下载模板重新上传");
                    return list;
                }

                foreach (var row in totalRows)
                {
                    var tnew = new T();
                    var errorInfoItem = new ValidatorErrorInfo
                    {
                        Row = row.ToString(),
                        ErrorDetails = new List<ErrorDetail>()
                    };

                    foreach (var col in columnPropertyInfos)
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
                                GetDataCheckError(errorInfoItem, col.Column, "数据格式不是数字");
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
                                GetDataCheckError(errorInfoItem, col.Column, "数据格式不是数字");
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
                              GetDataCheckError(errorInfoItem, col.Column, "数据格式不是数字");
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
                              GetDataCheckError(errorInfoItem, col.Column, "数据格式不是数字");
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
                               GetDataCheckError(errorInfoItem, col.Column, "数据格式不是数字");
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
                                GetDataCheckError(errorInfoItem, col.Column, "数据格式不是日期类型");
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
                              GetDataCheckError(errorInfoItem, col.Column, "含有特殊字符");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<String>());
                            }
                        }
                    }

                    //返回结果
                    list.Add(tnew);
                    //错误信息
                    errorInfo.Add(errorInfoItem);
                }
            #endregion

              return list;
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        private static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 返回excel错误详情
        /// </summary>
        /// <param name="errorInfo"></param>
        /// <param name="errorMsg"></param>
        private static void GetExcelDetailError(List<ValidatorErrorInfo> errorInfo,string errorMsg)
        {
            errorInfo.Add(new ValidatorErrorInfo
            {
                ErrorDetails = new List<ErrorDetail>
                {
                    new ErrorDetail{ErrorMsg = errorMsg}
                }
            });
        }

        /// <summary>
        /// 获取数据校验错误
        /// </summary>
        /// <param name="validatorErrorInfo"></param>
        /// <param name="col"></param>
        /// <param name="errorMsg"></param>
        private static void GetDataCheckError(ValidatorErrorInfo validatorErrorInfo, int col,string errorMsg)
        {
            validatorErrorInfo.ErrorDetails.Add(new ErrorDetail
            {
                Colum = col.ToString(),
                ErrorMsg = errorMsg
            });
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
