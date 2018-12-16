using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyAbpDemo.Infrastructure
{
    public static class ValidatorErrorInfoExtensions
    {
        /// <summary>
        /// 添加验证错误
        /// </summary>
        /// <param name="list">对象集合</param>
        /// <param name="errorMsg">错误信息</param>
        public static void AddValidatorErrorItem(this List<ValidatorErrorInfo> list,string errorMsg)
        {
            list.Add(new ValidatorErrorInfo
            {
                ErrorDetails = new List<ErrorDetail>
                {
                    new ErrorDetail{ErrorMsg = errorMsg}
                }
            });
        }

        /// <summary>
        /// 添加验证明细错误
        /// </summary>
        /// <param name="validatorErrorInfo">对象</param>
        /// <param name="col">列</param>
        /// <param name="errorMsg">错误信息</param>
        public static void AddValidatorErrorItem(this ValidatorErrorInfo validatorErrorInfo, int col,string errorMsg)
        {
            validatorErrorInfo.ErrorDetails.Add(new ErrorDetail
            {
                Column = col.ToString(),
                ErrorMsg = errorMsg
            });
        }

        /// <summary>
        /// 返回验证错误信息字符串(以分号隔开)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetValidatorErrorStr(this List<ValidatorErrorInfo> list)
        {
            //excel模板异常直接返回
            if (list.Count == 1 && string.IsNullOrEmpty(list.First().Row))
            {
                return list.Select(a => a.ErrorDetails.Select(b => b.ErrorMsg).First()).First();
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in list)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("第{0}行: ", item.Row);
                foreach (var errorDetails in item.ErrorDetails)
                {
                    sb.AppendFormat("第{0}列{1},", errorDetails.Column, errorDetails.ErrorMsg);
                }

                stringBuilder.AppendFormat("{0};", sb.ToString().TrimEnd(','));
            }

            return stringBuilder.ToString().TrimEnd(';');
        }
    }
}
