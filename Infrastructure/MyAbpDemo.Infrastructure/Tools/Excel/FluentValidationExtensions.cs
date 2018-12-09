using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace MyAbpDemo.Infrastructure
{
    public static class FluentValidationExtensions
    {
        /// <summary>
        /// 导入数据时检验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<ValidatorErrorInfo> GetObjectValidatorError<T>(this AbstractValidator<T> validator, List<T> list)
        {
            List<ValidatorErrorInfo> errorList = new List<ValidatorErrorInfo>();
            var propertyNames = typeof(T).GetProperties().Select(a => a.Name).ToList();

            foreach (var item in list)
            {
                ValidationResult results = validator.Validate(item);
                if (!results.IsValid)
                {
                    var errorInfoItem = new ValidatorErrorInfo
                    {
                        Row = (list.IndexOf(item) + 1).ToString(),
                        ErrorDetails = new List<ErrorDetail>()
                    };

                    foreach (var errorInfo in results.Errors)
                    {
                        errorInfoItem.ErrorDetails.Add
                        (
                            new ErrorDetail
                            {
                                Column = (propertyNames.IndexOf(errorInfo.PropertyName)+1).ToString(),
                                ErrorMsg = errorInfo.ErrorMessage
                            }
                        );
                    }

                    errorList.Add(errorInfoItem);
                }
            }

            return errorList;
        }
    }
}
