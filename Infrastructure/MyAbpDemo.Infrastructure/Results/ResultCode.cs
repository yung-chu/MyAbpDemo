using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyAbpDemo.Infrastructure
{
    /// <summary>
    /// 返回结果状态码
    /// </summary>
    public enum ResultCode
    {
        Undefined = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [Display(Name = "成功")]
        Ok = 1,

        /// <summary>
        /// 失败
        /// </summary>
        [Display(Name = "失败")]
        Fail = 100,

        /// <summary>
        /// 参数验证失败
        /// </summary>
        [Display(Name = "参数验证失败")]
        ParameterFailed = 101,

        /// <summary>
        /// 失败后刷新
        /// </summary>
        [Display(Name = "失败后刷新")]
        FailAndRefresh = 102,

        /// <summary>
        /// 登陆失败
        /// </summary>
        [Display(Name = "登陆失败")]
        LoginFailed = 200,

        /// <summary>
        /// 用户名错误
        /// </summary>
        [Display(Name = "用户名错误")]
        UserNameError = 201,

        /// <summary>
        /// 密码错误
        /// </summary>
        [Display(Name = "密码错误")]
        PasswordError = 202,

        /// <summary>
        /// 账号未激活
        /// </summary>
        [Display(Name = "账号未激活")]
        UserIsNotActive = 203,

        /// <summary>
        /// 账号被锁定
        /// </summary>
        [Display(Name = "账号被锁定")]
        LockedOut = 204,

        /// <summary>
        /// 授权失败
        /// </summary>
        [Display(Name = "授权失败")]
        Unauthorized = 206,

        /// <summary>
        /// 未授权，需代理授权
        /// </summary>
        [Display(Name = "需代理授权")]
        ProxyAuthorized = 207,

        /// <summary>
        /// 资源不存在
        /// </summary>
        [Display(Name = "资源不存在")]
        NotFound = 400,

        /// <summary>
        /// 重复插入
        /// </summary>
        [Display(Name = "重复插入")]
        DuplicateRecord = 401,

        /// <summary>
        /// 数据冲突
        /// </summary>
        [Display(Name = "数据冲突")]
        ConcurrencyRecord = 402,

        /// <summary>
        /// 无效的操作
        /// </summary>
        [Display(Name = "无效的操作")]
        InvalidOperation = 500
    }
}
