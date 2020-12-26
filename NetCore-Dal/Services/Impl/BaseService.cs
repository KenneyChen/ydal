using NetCore.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YDal.Repository;
using YDal.Repository.Impl;
using YDal.UnitOfWork;

namespace NetCore.Dal.Services.Impl
{

    /// <summary>
    /// service增强类
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseService<TRepository, TEntity>
        where TRepository : class, IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 直接注入类对象，获取数据库基本操作
        /// </summary>
        private readonly TRepository repository;

        public BaseService(
           TRepository TRepository
           )
        {
            this.repository = TRepository;
        }

        /// <summary>
        /// 获取操作对象
        /// </summary>
        public BaseEfRepository<TEntity> RepositoryImpl
        {
            get
            {
                if (repository is BaseEfRepository<TEntity>)
                {
                    return repository as BaseEfRepository<TEntity>;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取工作单元对象，批量提交使用
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get { return RepositoryImpl != null ? RepositoryImpl.unitOfWork : null; }
        }


        #region 公共方法

        /// <summary>
        ///     插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Insert(TEntity entity, bool isSave = true)
        {
            return this.repository.Insert(entity, isSave);
        }

        /// <summary>
        /// 批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Insert(IEnumerable<TEntity> entities, bool isSave = true)
        {
            return this.repository.Insert(entities, isSave);
        }


        /// <summary>
        /// 删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Delete(TEntity entity, bool isSave = true)
        {
            return this.repository.Delete(entity, isSave);
        }

        /// <summary>
        /// 删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Delete(IEnumerable<TEntity> entities, bool isSave = true)
        {
            return this.repository.Delete(entities, isSave);
        }

        /// <summary>
        /// 删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Delete(Expression<Func<TEntity, bool>> predicate, bool isSave = true)
        {
            return this.repository.Delete(predicate, isSave);
        }

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Update(TEntity entity, bool isSave = true)
        {
            return this.repository.Update(entity, isSave);
        }

        public int Update(IEnumerable<TEntity> entities, bool isSave = true)
        {
            return this.repository.Update(entities, isSave);
        }

        /// <summary>
        /// 提交更新
        /// </summary>
        /// <returns></returns>
        public int Commit(bool validateOnSaveEnabled = true) 
        {
            return this.repository.Commit(validateOnSaveEnabled);
        }


        /// <summary>
        /// 查找指定主键的实体记录(从缓存中获取)
        /// </summary>
        /// <param name="key"> 指定主键 </param>
        /// <returns> 符合编号的记录，不存在返回null </returns>
        public TEntity SelectById(object key)
        {
            return this.repository.SelectById(key);
        }

        /// <summary>
        /// 通过条件获取单条实体记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TEntity Filter(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return this.repository.Filter(filter, orderBy,includeProperties);
        }

        /// <summary>
        /// 通过条件获取单条实体记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TEntity FilterWithTracking(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return this.repository.FilterWithTracking(filter, orderBy, includeProperties);
        }

        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            return this.repository.Delete(predicate);
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="funWhere">查询条件-谓语表达式</param>
        /// <param name="funUpdate">实体-谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public int Update(Expression<Func<TEntity, bool>> funWhere, Expression<Func<TEntity, TEntity>> funUpdate)
        {
            return this.repository.Update(funWhere, funUpdate);
        }


        /// <summary>
        /// 执行非查询sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">事务属性</param>
        /// <returns>返回影响的行数</returns>
        public List<T> ExcuteQuery<T>(string sql, params object[] parameters)
        {
            return this.repository.ExcuteQuery<T>(sql, parameters);
        }

        /// <summary>
        /// 指定使用索引
        /// </summary>
        /// <param name="indexName"></param>
        public IQueryable<TEntity> UseIndex(string indexName)
        {
            return this.repository.UseIndex(indexName);
        }
        #endregion
    }
}
