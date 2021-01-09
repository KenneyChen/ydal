using Microsoft.EntityFrameworkCore;
using YDal.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace YDal.EntityFramework
{
    public class DalDbContext : DbContext
    {
        //public DbSet<Demo> Demo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public DalDbContext(DbContextOptions<DalDbContext> options) : base(options)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                foreach (var type in item.GetTypes())
                {
                    if (type.IsClass && type != typeof(BaseEntity) && typeof(BaseEntity).IsAssignableFrom(type))
                    {
                        modelBuilder.Entity(type);
                        ////var method = modelBuilder.GetType().GetMethods().Where(x => x.Name == "Entity").FirstOrDefault();
                        ////if (method != null)
                        ////{
                        ////    method = method.MakeGenericMethod(new Type[] { type });
                        ////    method.Invoke(modelBuilder, null);
                        ////}
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        ///     注册一个新的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>
        /// <param name="entity"> 要注册的对象 </param>
        public void RegisterNew<TEntity>(TEntity entity) where TEntity : class// EntityBase<TKey>
        {
            EntityState state = this.Entry(entity).State;
            if (state == EntityState.Detached)
            {
                this.Entry(entity).State = EntityState.Added;
            }
        }

        /// <summary>
        ///     批量注册多个新的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>
        /// <param name="entities"> 要注册的对象集合 </param>
        public void RegisterNew<TEntity>(IEnumerable<TEntity> entities) where TEntity : class// EntityBase<TKey>
        {
            foreach (TEntity entity in entities)
            {
                RegisterNew<TEntity>(entity);
            }

        }

        public void RegisterModified<TEntity>(IEnumerable<TEntity> entities) where TEntity : class// EntityBase<TKey>
        {
            foreach (TEntity entity in entities)
            {
                EntityState state = this.Entry(entity).State;
                if (state == EntityState.Detached)
                {
                    this.Entry(entity).State = EntityState.Modified;
                }
            }
        }

        /// <summary>
        ///     注册一个更改的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>
        /// <param name="entity"> 要注册的对象 </param>
        public void RegisterModified<TEntity>(TEntity entity) where TEntity : class// EntityBase<TKey>
        {
            this.Update<TEntity>(entity);
        }

        /// <summary>
        /// 使用指定的属性表达式指定注册更改的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity">要注册的类型</typeparam>
        /// <param name="propertyExpression">属性表达式，包含要更新的实体属性</param>
        /// <param name="entity">附带新值的实体信息，必须包含主键</param>
        public void RegisterModified<TEntity>(Expression<Func<TEntity, object>> propertyExpression, TEntity entity) where TEntity : class// EntityBase<TKey>
        {
            //Context.Update<TEntity>(propertyExpression, entity);
            //IsCommitted = false;
        }

        /// <summary>
        ///   注册一个删除的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>
        /// <param name="entity"> 要注册的对象 </param>
        public void RegisterDeleted<TEntity>(TEntity entity) where TEntity : class// EntityBase<TKey>
        {
            this.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        ///   批量注册多个删除的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>
        /// <param name="entities"> 要注册的对象集合 </param>
        public void RegisterDeleted<TEntity>(IEnumerable<TEntity> entities) where TEntity : class// EntityBase<TKey>
        {
            foreach (TEntity entity in entities)
            {
                RegisterDeleted<TEntity>(entity);
            }

        }
        /// <summary>
        ///   从仓储上下文中删除注册的对象
        /// </summary>
        /// <typeparam name="TEntity"> 要删除注册的类型 </typeparam>
        /// <param name="entity"> 要删除的对象 </param>
        public void RegisterRemove<TEntity>(TEntity entity) where TEntity : class// EntityBase<TKey>
        {
            this.Set<TEntity>().Remove(entity);
        }

    }
}
