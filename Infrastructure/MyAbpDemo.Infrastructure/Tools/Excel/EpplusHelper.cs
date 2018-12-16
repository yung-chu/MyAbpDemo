using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MyAbpDemo.Infrastructure
{
    public class EpplusHelper
    {     
        //excel导出错误单元格字段
        public  const string ExportErrorCell= "ErrorMessage";

        /// <summary>
        /// 使用EPPlus导出Excel(xlsx)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="list">对象数据</param>
        /// <param name="fileInfo">文件对象</param>
        /// <param name="sheetName">单元</param>
        public static void Export<T>(IEnumerable<T> list, FileInfo fileInfo,List<CellPosition> cellPositions, string sheetName = "sheet1") where T : new()
        {
            //表头
            var propertyInfos = typeof(T).GetProperties().ToList();
            var objectColumns = propertyInfos.Select(a => a.Name);

            using (ExcelPackage pck = new ExcelPackage(fileInfo))
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);
               
                //Load the IEnumerable<T> into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromCollection(list, true);

                //设置列格式
                //https://q.cnblogs.com/q/65222/
                foreach (var item in propertyInfos)
                {
                    var col = propertyInfos.IndexOf(item) + 1;
                    switch (item.PropertyType.Name.ToLower())
                    {
                        case "decimal":
                            ws.Column(col).Style.Numberformat.Format = "#,##0.00";//保留两位小数
                            break;

                        case "datetime":
                            ws.Column(col).Style.Numberformat.Format = "yyyy-mm-dd";
                            break;
                    }
                }

                //获取一个区域，并对该区域进行样式设置
                ExcelBorderStyle borderStyle = ExcelBorderStyle.Thin;
                Color borderColor = Color.FromArgb(155, 155, 155);
                //对所有区域设置 ExcelRange(ExcelWorksheet sheet, int fromRow, int fromCol, int toRow, int toCol)
                using (ExcelRange rng = ws.Cells[1, 1, list.Count() + 1, objectColumns.Count()])
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
                using (ExcelRange rng = ws.Cells[1, 1, 1, objectColumns.Count()])
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
                        if (!string.IsNullOrEmpty(objectPropertyValue))
                        {
                            int row = getList.IndexOf(item) + 1;
                            using (ExcelRange rng = ws.Cells[row + 1, 1, row + 1, objectColumns.Count()])
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
                    using (ExcelRange rng = ws.Cells[item.FromRow,item.FromCol,item.ToRow,item.ToCol])
                    {
                        rng.Merge = true;
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center; //垂直居中
                    }
                }

                pck.Save();
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
            if (property == null) return string.Empty;
            object o = property.GetValue(t, null);
            if (o == null) return string.Empty;
            return o.ToString();
        }


        /// <summary>
        /// 使用EPPlus导入Excel(xlsx)
        /// 基本校验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uploadedFile">请求文件</param>
        /// <param name="errorMsg">基本校验错误列表</param>
        /// <returns></returns>
        public static List<T> Import<T>(IFormFile uploadedFile, List<ValidatorErrorInfo> errorMsg) where T : new()
        {
            using (ExcelPackage package = new ExcelPackage(uploadedFile.OpenReadStream()))
            {
                ExcelWorksheets workSheetList = package.Workbook.Worksheets;
                if (workSheetList.Any())//是否有sheet
                {
                    ExcelWorksheet workSheet = workSheetList[1];//取第一个sheet
                    return workSheet.ConvertSheetToObjects<T>(errorMsg);
                }
            }
            return new List<T>();
        }
    }

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
}
