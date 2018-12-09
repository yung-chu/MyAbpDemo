using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.Infrastructure
{
    /// <summary>
    /// excel 错误校验信息
    /// </summary>
    public class ValidatorErrorInfo
    {
        /// <summary>
        /// 行
        /// </summary>
        public string Row { get; set; }

        /// <summary>
        /// 详细列错误信息
        /// </summary>
        public List<ErrorDetail> ErrorDetails { get; set; }
    }

    public class ErrorDetail
    {
        /// <summary>
        /// 列
        /// </summary>
        public string Column{ get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
}
