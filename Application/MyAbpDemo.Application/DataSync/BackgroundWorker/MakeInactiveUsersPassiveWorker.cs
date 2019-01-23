using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using MyAbpDemo.Core;

namespace MyAbpDemo.Application
{
    /// <summary>
    /// 后台工人是不同于后台作业的。在应用程序中，它们是运行在后台单个线程中。
    /// 一般来说，它们周期性的执行一些任务
    /// </summary>
    public class MakeInactiveUsersPassiveWorker:PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IStudentRepository _studentRepository;

        public MakeInactiveUsersPassiveWorker(AbpTimer timer, IStudentRepository studentRepository) : base(timer)
        {
            Timer.Period = 5000; //5 seconds (good for tests, but normally will be more)
            _studentRepository = studentRepository;
        }

        /// <summary>
        /// 将不活跃用户标记为禁用
        /// </summary>
        protected override void DoWork()
        {
            var oneMonthAgo = Clock.Now.Subtract(TimeSpan.FromDays(30));
            var inactiveUsers = _studentRepository.GetAllList(a =>
                a.IsActive && (a.LastModificationTime.HasValue && a.LastModificationTime < oneMonthAgo) ||
                    (a.CreationTime < oneMonthAgo && a.LastModificationTime == null));

            foreach (var item in inactiveUsers)
            {
                 item.IsActive = false;
                 _studentRepository.Update(item);
            }
        }
    }
}
