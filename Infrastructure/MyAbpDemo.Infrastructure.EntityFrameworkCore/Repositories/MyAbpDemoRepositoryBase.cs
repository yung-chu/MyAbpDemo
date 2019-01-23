using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;


namespace MyAbpDemo.Infrastructure.EFCore
{
    //https://github.com/ABPFrameWorkGroup/AbpDocument2Chinese/blob/master/Markdown/Abp/9.3ABP%E5%9F%BA%E7%A1%80%E8%AE%BE%E6%96%BD%E5%B1%82-%E9%9B%86%E6%88%90EntityFrameworkCore.md#%E8%87%AA%E5%AE%9A%E4%B9%89%E4%BB%93%E5%82%A8
    public class MyAbpDemoRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<MyAbpDemoDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public MyAbpDemoRepositoryBase(IDbContextProvider<MyAbpDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        #region   为所有仓储添加通用方法
        /// <summary>
        /// 放弃实体的所有修改操作
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Unchanged(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Unchanged;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void AddRange(IEnumerable<TEntity> entities)
        {
            Table.AddRange(entities);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            Table.UpdateRange(entities);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Table.RemoveRange(entities);
        }
        #endregion


    }

    //对于那些有int类型Id的实体的仓储的快速实现方式
    public class MyAbpDemoRepositoryBase<TEntity> : MyAbpDemoRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        public MyAbpDemoRepositoryBase(IDbContextProvider<MyAbpDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        //别在这里添加任何方法，请将方法添加到上面那个类，因为该类被上面类继承
    }
}
