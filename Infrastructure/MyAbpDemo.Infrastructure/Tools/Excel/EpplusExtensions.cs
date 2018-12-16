using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MyAbpDemo.Infrastructure
{
    /// <summary>
    /// EPPLus相关扩展类
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

        /// <summary>
        /// excel导出错误单元格字段
        /// </summary>
        public const string ExportErrorCell = "ErrorMessage";

        /// <summary>
        /// 导入时将表格转换为数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="worksheet"></param>
        /// <param name="errorInfo"></param>
        /// <returns></returns>
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
                    var columnName = displayNameAttribute == null ? String.Empty : ((DisplayNameAttribute)displayNameAttribute).DisplayName.Trim();

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
                    errorInfo.AddValidatorErrorItem("excel无数据,请下载模板重新上传");
                    return list;
                }
                else if (!totalRows.Any())
                {
                    errorInfo.AddValidatorErrorItem("excel无数据,请重新上传");
                    return list;
                }
                if (totalRows.First() > MaxRows)
                {
                    errorInfo.AddValidatorErrorItem($"数据量过大,超过{ExcelPackage.MaxRows},请重新上传");
                    return list;
                }
                if (columnNames.Count > MaxColumns)
                {
                    errorInfo.AddValidatorErrorItem($"列名太长,超过{ExcelPackage.MaxColumns},请重新上传");
                    return list;
                }
                if (!columnPropertyInfos.Select(a => a.ColumnName).SequenceEqual(columnNames))
                {
                    errorInfo.AddValidatorErrorItem("excel标题与模板标题不一致，请下载模板重新上传");
                    return list;
                }

                foreach (var row in totalRows)
                {
                    var tnew = new T();
                    var errorInfoItem = new ValidatorErrorInfo
                    {
                        Row = (row-1).ToString(),
                        ErrorDetails = new List<ErrorDetail>()
                    };

                    foreach (var col in columnPropertyInfos)
                    {
                        var val = worksheet.Cells[row, col.Column];

                        if (val.Value == null)//数据为空默认空字符
                        {
                            val.Value = String.Empty;
                        }
                        if (col.Property.PropertyType == typeof(short))
                        {
                            if (!Int16.TryParse(val.Value.ToString(), out short a))
                            {
                                errorInfoItem.AddValidatorErrorItem(col.Column, "数据格式不是数字");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<short>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(Int32))
                        {
                            //空字符默认值0
                            if (String.IsNullOrEmpty(val.Value.ToString()))
                            {
                                val.Value = 0;
                            }

                            if (!Int32.TryParse(val.Value.ToString(), out int a))
                            {
                                errorInfoItem.AddValidatorErrorItem(col.Column, "数据格式不是数字");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<int>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(long))
                        {
                            if (!Int64.TryParse(val.Value.ToString(), out long a))
                            {
                              errorInfoItem.AddValidatorErrorItem(col.Column, "数据格式不是数字");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<long>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(decimal))
                        {
                            if (!Decimal.TryParse(val.Value.ToString(), out decimal a))
                            {
                              errorInfoItem.AddValidatorErrorItem(col.Column, "数据格式不是数字");
                            }
                            else
                            {
                                col.Property.SetValue(tnew, val.GetValue<decimal>());
                            }
                        }
                        if (col.Property.PropertyType == typeof(float))
                        {
                            if (!Single.TryParse(val.Value.ToString(), out float a))
                            {
                            errorInfoItem.AddValidatorErrorItem(col.Column, "数据格式不是数字");
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
                                errorInfoItem.AddValidatorErrorItem(col.Column, "数据格式不是日期类型");
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
                                errorInfoItem.AddValidatorErrorItem(col.Column, "含有特殊字符");
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

               //错误信息处理(移除创建多余实例对象)        
               errorInfo.RemoveAll(a => !a.ErrorDetails.Any());
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
        /// 导出时填充数据到表格
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excelPackage"></param>
        /// <param name="cellPositions"></param>
        /// <param name="sheetName"></param>
        public static void ConvertObjectsToSheet<T>(this ExcelPackage excelPackage, IEnumerable<T> list, List<CellPosition> cellPositions, string sheetName = "sheet1")
        {
            //表头
            var propertyInfos = typeof(T).GetProperties().ToList();
            var objectColumns = propertyInfos.Select(a => a.Name);

            //Create the worksheet
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);

            //Load the IEnumerable<T> into the sheet, starting from cell A1. Print the column names on row 1
            worksheet.Cells["A1"].LoadFromCollection(list, true);

            //设置列格式
            //https://q.cnblogs.com/q/65222/
            foreach (var item in propertyInfos)
            {
                var col = propertyInfos.IndexOf(item) + 1;
                switch (item.PropertyType.Name.ToLower())
                {
                    case "decimal":
                        worksheet.Column(col).Style.Numberformat.Format = "#,##0.00";//保留两位小数
                        break;

                    case "datetime":
                        worksheet.Column(col).Style.Numberformat.Format = "yyyy-mm-dd";
                        break;
                }
            }

            //获取一个区域，并对该区域进行样式设置
            ExcelBorderStyle borderStyle = ExcelBorderStyle.Thin;
            Color borderColor = Color.FromArgb(155, 155, 155);
            //对所有区域设置 ExcelRange(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol)
            using (ExcelRange rng = worksheet.Cells[1, 1, list.Count() + 1, objectColumns.Count()])
            {
                rng.Style.Font.Name = "宋体";
                rng.Style.Font.Size = 10;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;    //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                rng.Style.Border.Top.Style = borderStyle;
                rng.Style.Border.Top.Color.SetColor(borderColor);

                rng.Style.Border.Bottom.Style = borderStyle;
                rng.Style.Border.Bottom.Color.SetColor(borderColor);

                rng.Style.Border.Right.Style = borderStyle;
                rng.Style.Border.Right.Color.SetColor(borderColor);
            }

            //Format the header row
            using (ExcelRange rng = worksheet.Cells[1, 1, 1, objectColumns.Count()])
            {
                rng.Style.Font.Bold = true;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 246));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.FromArgb(51, 51, 51));
            }

            //导出时返回校验错误行标红
            //字段必须是 ExportErrorCell=ErrorMessage
            var getList = list.ToList();
            bool isExistsError = objectColumns.Contains(ExportErrorCell);
            if (isExistsError)
            {
                foreach (var item in list)
                {
                    var objectPropertyValue = GetObjectPropertyValue(item, ExportErrorCell);
                    if (!String.IsNullOrEmpty(objectPropertyValue))
                    {
                        int row = getList.IndexOf(item) + 1;
                        using (ExcelRange rng = worksheet.Cells[row + 1, 1, row + 1, objectColumns.Count()])
                        {
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rng.Style.Fill.BackgroundColor.SetColor(Color.OrangeRed);
                        }
                    }
                }
            }

            //合并单元格
            foreach (var item in cellPositions)
            {
                using (ExcelRange rng = worksheet.Cells[item.FromRow, item.FromCol, item.ToRow, item.ToCol])
                {
                    rng.Merge = true;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center; //垂直居中
                }
            }
        }

        /// <summary>
        /// 根据属性名获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        private static string GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(propertyname);
            if (property == null) return String.Empty;
            object o = property.GetValue(t, null);
            if (o == null) return String.Empty;
            return o.ToString();
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

    /// <summary>
    /// 单元格位置
    /// </summary>
    public class CellPosition
    {
        /// <summary>
        /// 开始行
        /// </summary>
        public int FromRow { get; set; }

        /// <summary>
        /// 开始列
        /// </summary>
        public int FromCol { get; set; }

        /// <summary>
        /// 结束行
        /// </summary>
        public int ToRow { get; set; }

        /// <summary>
        /// 结束列
        /// </summary>
        public int ToCol { get; set; }
    }

    /// <summary>
    /// 导出参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExportSheet<T>
    {        
        /// <summary>
        /// 表单名
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 单元格位置
        /// </summary>
        public List<CellPosition> CellPositions { get; set; }
    }
}
