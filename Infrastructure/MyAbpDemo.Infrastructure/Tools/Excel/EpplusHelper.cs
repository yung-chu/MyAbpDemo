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
        /// 两个sheet
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
        /// 标准导出
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
        /// Excel导入两个workSheet(xlsx)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <param name="uploadedFile">请求文件</param>
        /// <param name="errorMsgOne">基本校验错误列表</param>
        /// <param name="errorMsgTwo">基本校验错误列表</param>
        /// <returns></returns>
        public static Tuple<List<T>, List<TS>> Import<T, TS>(IFormFile uploadedFile, List<ValidatorErrorInfo> errorMsgOne, List<ValidatorErrorInfo> errorMsgTwo) where T : new() where TS : new()
        {
            using (ExcelPackage package = new ExcelPackage(uploadedFile.OpenReadStream()))
            {
                ExcelWorksheets workSheetList = package.Workbook.Worksheets;
                if (workSheetList.Any() && workSheetList.Count >= 2)
                {
                    var workSheetOne = workSheetList[1].ConvertSheetToObjects<T>(errorMsgOne);
                    var workSheetTwo = workSheetList[2].ConvertSheetToObjects<TS>(errorMsgTwo);
                    return Tuple.Create(workSheetOne, workSheetTwo);
                }
            }
            return new Tuple<List<T>, List<TS>>(new List<T>(), new List<TS>());
        }

        /// <summary>
        /// 标准使用EPPlus导入Excel(xlsx)
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
