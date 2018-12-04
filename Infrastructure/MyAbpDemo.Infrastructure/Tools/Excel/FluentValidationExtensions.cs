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
        /// 导入数据检验
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static StringBuilder GetObjectValidatorError<T>(this AbstractValidator<T> validator, List<T> list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in list)
            {
                int currentRow = list.IndexOf(item) + 1;
                ValidationResult results = validator.Validate(item);

                if (!results.IsValid)
                {
                    results.Errors.Select(a => a.ErrorMessage).ToList().ForEach(errorMsg =>
                    {
                        stringBuilder.Append($"第{currentRow}行:{errorMsg};");
                    });
                }
            }

            return stringBuilder;
        }
    }
}
