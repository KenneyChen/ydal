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
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {

        private readonly IUnitOfWork unitOfWork;
        /// <summary>
        /// 直接注入类对象，获取数据库基本操作
        /// </summary>
        private readonly IRepository<TEntity> repository;

        public BaseService(IRepository<TEntity> _repository, IUnitOfWork _unitOfWork)
        {
            this.repository = _repository;
            this.unitOfWork = _unitOfWork;
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
            this.repository.Insert(entity);
            return isSave ? 0 : unitOfWork.Commit();
        }

        /// <summary>
        /// 批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Insert(IEnumerable<TEntity> entities, bool isSave = true)
        {
            this.repository.Insert(entities);
            return isSave ? 0 : unitOfWork.Commit();
        }


        /// <summary>
        /// 删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Delete(TEntity entity, bool isSave = true)
        {
            this.repository.Delete(entity);
            return isSave ? 0 : unitOfWork.Commit();
        }

        /// <summary>
        /// 删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Delete(IEnumerable<TEntity> entities, bool isSave = true)
        {
            this.repository.Delete(entities);
            return isSave ? 0 : unitOfWork.Commit();
        }

        /// <summary>
        /// 删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Delete(Expression<Func<TEntity, bool>> predicate, bool isSave = true)
        {
            this.repository.Delete(predicate);

            return isSave ? 0 : unitOfWork.Commit();
        }

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public int Update(TEntity entity, bool isSave = true)
        {
            this.repository.Update(entity);
            return isSave ? 0 : unitOfWork.Commit();
        }

        public int Update(IEnumerable<TEntity> entities, bool isSave = true)
        {
            this.repository.Update(entities);

            return isSave ? 0 : unitOfWork.Commit();
        }

        /// <summary>
        /// 提交更新
        /// </summary>
        /// <returns></returns>
        public int Commit(bool validateOnSaveEnabled = true)
        {
            return this.unitOfWork.Commit(validateOnSaveEnabled);
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
            return this.repository.Filter(filter, orderBy, includeProperties);
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
            this.repository.Delete(predicate);
            return unitOfWork.Commit();
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
