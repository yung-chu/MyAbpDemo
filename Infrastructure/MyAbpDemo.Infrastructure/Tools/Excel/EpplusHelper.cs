using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MyAbpDemo.Infrastructure
{
    public class EpplusHelper
    {     
        /// <summary>
        /// 使用EPPlus导出Excel(xlsx)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="list">对象数据</param>
        /// <param name="fileInfo">文件对象</param>
        /// <param name="sheetName">单元</param>
        public static void Export<T>(IEnumerable<T> list, FileInfo fileInfo, string sheetName = "sheet1") where T : new()
        {
            //表头
            var propertyInfos = typeof(T).GetProperties().ToList();
            var columnCount = propertyInfos.Select(a => a.Name).Count();

            using (ExcelPackage pck = new ExcelPackage(fileInfo))
            {
                //Create the worksheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);
               
                //Load the IEnumerable<T> into the sheet, starting from cell A1. Print the column names on row 1
                ws.Cells["A1"].LoadFromCollection(list, true);


                //设置列格式
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

                using (ExcelRange rng = ws.Cells[1, 1, list.Count() + 1, columnCount])
                {
       
                    rng.Style.Font.Name = "宋体";
                    rng.Style.Font.Size = 10;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;    //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    rng.Style.Border.Top.Style = borderStyle;
                    rng.Style.Border.Top.Color.SetColor(borderColor);

                    rng.Style.Border.Bottom.Style = borderStyle;
                    rng.Style.Border.Bottom.Color.SetColor(borderColor);

                    rng.Style.Border.Right.Style = borderStyle;
                    rng.Style.Border.Right.Color.SetColor(borderColor);
                }

                //Format the header row
                using (ExcelRange rng = ws.Cells[1, 1, 1, columnCount])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 246));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(Color.FromArgb(51, 51, 51));
                }

                pck.Save();
            }
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
}
