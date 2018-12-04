using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.Infrastructure
{
    interface IDataResult<T>
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        T Data { get; set; }
    }
}
