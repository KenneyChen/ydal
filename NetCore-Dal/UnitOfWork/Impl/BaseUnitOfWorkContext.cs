using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;

namespace YDal.UnitOfWork.Impl
{
    /// <summary>
    ///     单元操作实现基类
    /// </summary>
    public abstract class BaseUnitOfWorkContext : IUnitOfWorkContext
    {
        /// <summary>
        /// 获取 当前使用的数据访问上下文对象
        /// </summary>
        protected abstract DbContext Context { get; }

        /// <summary>
        ///     获取 当前单元操作是否已被提交
        /// </summary>
        public bool IsCommitted { get; private set; }

        public DbContext DbContext { get { return Context; } }

        /// <summary>
        ///     提交当前单元操作的结果
        /// </summary>
        /// <param name="validateOnSaveEnabled">保存时是否自动验证跟踪实体</param>
        /// <returns></returns>
        public int Commit(bool validateOnSaveEnabled = false)
        {
            //validateOnSaveEnabled=true时莫名其妙报错，暂时去掉
            validateOnSaveEnabled = false;
            if (IsCommitted)
            {
                return 0;
            }
            try
            {
                int result = Context.SaveChanges(validateOnSaveEnabled);
                IsCommitted = true;
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ;
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.InnerException is SqlException)
                {
                    var sqlEx = e.InnerException.InnerException as SqlException;
                    //string msg = DataHelper.GetSqlExceptionMessage(sqlEx.Number);
                    throw sqlEx; //PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + msg, sqlEx);
                }
                throw;
            }

        }

        /// <summary>
        ///  把当前单元操作标记成未提交状态,发生错误时，不能回滚事务，不要调用!
        /// </summary>
        public void Rollback()
        {
            IsCommitted = false;
        }

        public void Dispose()
        {
            //if (!IsCommitted)
            //{
            //    Commit();
            //}
            //if (Context != null)
            //    Context.Dispose();
        }

        /// <summary>
        ///   为指定的类型返回 System.Data.Entity.DbSet，这将允许对上下文中的给定实体执行 CRUD 操作。
        /// </summary>
        /// <typeparam name="TEntity"> 应为其返回一个集的实体类型。 </typeparam>
        /// <returns> 给定实体类型的 System.Data.Entity.DbSet 实例。 </returns>
        public DbSet<TEntity> Set<TEntity>() where TEntity : class// EntityBase<TKey>
        {
            return Context.Set<TEntity>();
        }

        /// <summary>
        ///     注册一个新的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>
        /// <param name="entity"> 要注册的对象 </param>
        public void RegisterNew<TEntity>(TEntity entity) where TEntity : class// EntityBase<TKey>
        {
            EntityState state = Context.Entry(entity).State;
            if (state == EntityState.Detached)
            {
                Context.Entry(entity).State = EntityState.Added;
            }
            IsCommitted = false;
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
                    EntityState state = Context.Entry(entity).State;
                    if (state == EntityState.Detached)
                    {
                        Context.Entry(entity).State = EntityState.Modified;
                    }
                    IsCommitted = false;
                }
           
        }

        /// <summary>
        ///     注册一个更改的对象到仓储上下文中
        /// </summary>
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>
        /// <param name="entity"> 要注册的对象 </param>
        public void RegisterModified<TEntity>(TEntity entity) where TEntity : class// EntityBase<TKey>
        {
            Context.Update<TEntity>(entity);
            IsCommitted = false;
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
            Context.Entry(entity).State = EntityState.Deleted;
            IsCommitted = false;
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
            Context.Set<TEntity>().Remove(entity);
            IsCommitted = false;
        }

        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet SqlQueryForDataSet(CommandType commandType, string sql, SqlParameter[] parameters)
        {
            using (var conn = this.DbContext.Database.GetDbConnection())
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                var cmd = new SqlCommand { Connection = (SqlConnection)conn, CommandText = sql, CommandType = commandType, CommandTimeout = conn.ConnectionTimeout };
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }

                var adapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                adapter.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }
    }
}
