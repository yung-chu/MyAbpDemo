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
        /// <summary>
        /// 使用EPPlus导出Excel(xlsx)
        /// </summary>
        /// <typeparam name="TS">对象类型1</typeparam>
        /// <typeparam name="T">对象类型2</typeparam>
        /// <param name="exportSheetOne">对象数据1</param>
        /// <param name="exportSheetTwo">对象数据2</param>
        /// <param name="fileInfo">文件对象</param>
        public static void Export<TS,T>(ExportSheet<TS> exportSheetOne, ExportSheet<T> exportSheetTwo, FileInfo fileInfo) 
        {
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                excelPackage.ConvertObjectsToSheet(exportSheetOne.Data, exportSheetOne.CellPositions, exportSheetOne.SheetName);
                excelPackage.ConvertObjectsToSheet(exportSheetTwo.Data, exportSheetTwo.CellPositions, exportSheetTwo.SheetName);
                excelPackage.Save();
            }
        }

        /// <summary>
        /// 单个sheet导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="cellPositions"></param>
        /// <param name="fileInfo"></param>
        /// <param name="sheetName"></param>
        public static void Export<T>(IEnumerable<T> data, FileInfo fileInfo, List<CellPosition> cellPositions,string sheetName= "sheet1") where T : new()
        {
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                excelPackage.ConvertObjectsToSheet(data, cellPositions, sheetName);
                excelPackage.Save();
            }
        }

        /// <summary>
        /// 导入Excel(xlsx)
        /// 包含多个Worksheets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uploadedFile">请求文件</param>
        /// <param name="errorMsg">基本校验错误列表</param>
        /// <returns></returns>
        public static List<List<T>> ImportMultipleSheets<T>(IFormFile uploadedFile, List<ValidatorErrorInfo> errorMsg) where T : new()
        {
            var list = new List<List<T>>();
            using (ExcelPackage excelPackage = new ExcelPackage(uploadedFile.OpenReadStream()))
            {
                ExcelWorksheets workSheetList = excelPackage.Workbook.Worksheets;
                foreach (var workSheet in workSheetList)
                {
                    list.Add(workSheet.ConvertSheetToObjects<T>(errorMsg));
                }
            }

            return list;
        }

        /// <summary>
        /// 使用EPPlus导入Excel(xlsx)
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
                if (workSheetList.Any())
                {
                    ExcelWorksheet workSheet = workSheetList[1];//取第一个sheet
                    return workSheet.ConvertSheetToObjects<T>(errorMsg);
                }
            }
            return new List<T>();
        }
    }
}
