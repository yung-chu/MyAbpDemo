using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Core;

namespace MyAbpDemo.Application
{
    /// <summary>
    /// https://aspnetboilerplate.com/Pages/Documents/Caching
    /// </summary>
    public static class ICacheManagerExtensions
    {
        private const string StudentCache = "StudentCache";

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public static GetStudentListOutput GetStudent(this ICacheManager cacheManager,string studentId)
        {
            return cacheManager.GetCache<string, GetStudentListOutput>(StudentCache).GetOrDefault(studentId);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="studentId"></param>
        /// <param name="student"></param>
        public static void SetStudent(this ICacheManager cacheManager, string studentId, GetStudentListOutput student)
        {
             cacheManager.GetCache<string, GetStudentListOutput>(StudentCache).Set(studentId, student,TimeSpan.FromHours(2));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="studentId"></param>
        public static void  RemoveStudent(this ICacheManager cacheManager, string studentId)
        {
            cacheManager.GetCache<string, GetStudentListOutput>(StudentCache).Remove(studentId);
        }



        private const string UserTokenKey = "UserTokenKey";

        /// <summary>
        /// 获取用户Token
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetUserTokenCache(this ICacheManager cacheManager, string userId)
        {
            return cacheManager.GetCache<string, string>(UserTokenKey).GetOrDefault(userId);
        }

        /// <summary>
        /// 设置用户Token缓存2小时
        /// </summary>
        /// <param name="cacheManager"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        public static void SetUserTokenCache(this ICacheManager cacheManager, string userId, string token)
        {
            cacheManager.GetCache<string, string>(UserTokenKey).Set(userId, token, TimeSpan.FromHours(2));
        }
    }
}
