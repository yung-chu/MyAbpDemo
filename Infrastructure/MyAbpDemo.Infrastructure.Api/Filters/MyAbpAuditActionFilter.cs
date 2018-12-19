using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Application.Services;
using Abp.Aspects;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.Auditing;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyAbpDemo.Infrastructure
{
    /// <summary>
    /// 源码分析
    /// https://www.cnblogs.com/myzony/p/9723531.html
    /// </summary>
    public class MyAbpAuditActionFilter : IAsyncActionFilter, ITransientDependency
    { 
       // 审计日志组件配置对象
        private readonly IAbpAspNetCoreConfiguration _configuration;
        // 真正用来写入审计日志的工具类
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditingConfiguration _auditingConfiguration;
       
        public IAbpSession AbpSession { get; set; }

        public MyAbpAuditActionFilter(IAbpAspNetCoreConfiguration configuration, IAuditingHelper auditingHelper, IAuditingConfiguration auditingConfiguration) 
        {
            _configuration = configuration;
            _auditingHelper = auditingHelper;
            _auditingConfiguration = auditingConfiguration;
            AbpSession = NullAbpSession.Instance;
        }

        public  async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldSaveAudit(context))
            {
                await next();
                return;
            }


            //using (AbpCrossCuttingConcerns.Applying(context.Controller, AbpCrossCuttingConcerns.Auditing))
            using (MyAbpCrossCuttingConcerns.Applying(context.Controller, MyAbpCrossCuttingConcerns.Auditing))
            {
                var auditInfo = _auditingHelper.CreateAuditInfo(
                    context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType(),
                    context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                    context.ActionArguments
                );

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    var result = await next();
                    if (result.Exception != null && !result.ExceptionHandled)
                    {
                        auditInfo.Exception = result.Exception;
                    }
                }
                catch (Exception ex)
                {
                    auditInfo.Exception = ex;
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                    await _auditingHelper.SaveAsync(auditInfo); //调用IAuditingStore SaveAsync

                }
            }
        }

        private bool ShouldSaveAudit(ActionExecutingContext actionContext)
        {
            return _configuration.IsAuditingEnabled &&
                   actionContext.ActionDescriptor.IsControllerAction() &&
                   ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true)
                 /*  _auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true)*/;
        }


        public bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false)
        {
            if (!_auditingConfiguration.IsEnabled)
            {
                return false;
            }

            //IsEnabledForAnonymousUsers:如果此值为true,那么没有登录到系统的用户的审计日志也会保存。默认为false。
            //if (!_auditingConfiguration.IsEnabledForAnonymousUsers && (AbpSession?.UserId == null))
            //{
            //    return false;
            //}

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
                {
                    return true;
                }

                if (classType.GetTypeInfo().IsDefined(typeof(DisableAuditingAttribute), true))
                {
                    return false;
                }

                if (_auditingConfiguration.Selectors.Any(selector => selector.Predicate(classType)))
                {
                    return true;
                }
            }

            return defaultValue;
        }

    }



    /// <summary>
    /// 参照源码 AbpCrossCuttingConcerns
    /// </summary>
    public class MyAbpCrossCuttingConcerns
    {
        public const string Auditing = "AbpAuditing";

        public static IDisposable Applying(object obj, params string[] concerns)
        {
            AddApplied(obj, concerns);
            return new DisposeAction(() =>
            {
                RemoveApplied(obj, concerns);
            });
        }

        public static void AddApplied(object obj, params string[] concerns)
        {
            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            (obj as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.AddRange(concerns);
        }

        public static void RemoveApplied(object obj, params string[] concerns)
        {
            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            var crossCuttingEnabledObj = obj as IAvoidDuplicateCrossCuttingConcerns;
            if (crossCuttingEnabledObj == null)
            {
                return;
            }

            foreach (var concern in concerns)
            {
                crossCuttingEnabledObj.AppliedCrossCuttingConcerns.RemoveAll(c => c == concern);
            }
        }
    }
}
