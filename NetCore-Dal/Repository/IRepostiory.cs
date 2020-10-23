using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace YDal.Repository
{
    public interface IRepository { }

    public interface IRepository<TEntity>: IRepository
    {
        #region 属性

        /// <summary>
        ///     获取 当前实体的查询数据集
        /// </summary>
        IQueryable<TEntity> Entities { get; }

        IQueryable<TEntity> Table { get; }

        #endregion

        #region 公共方法

        /// <summary>
        ///     插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Insert(TEntity entity, bool isSave = true);

        /// <summary>
        ///     批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Insert(IEnumerable<TEntity> entities, bool isSave = true);


        /// <summary>
        ///  删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Delete(TEntity entity, bool isSave = true);

        /// <summary>
        ///     删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Delete(IEnumerable<TEntity> entities, bool isSave = true);

        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Delete(Expression<Func<TEntity, bool>> predicate, bool isSave = true);

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Update(TEntity entity, bool isSave = true);

        int Update(IEnumerable<TEntity> entities, bool isSave = true);

        /// <summary>
        /// 提交更新
        /// </summary>
        /// <returns></returns>
        int Commit(bool validateOnSaveEnabled = true);

    
        /// <summary>
        /// 查找指定主键的实体记录(从缓存中获取)
        /// </summary>
        /// <param name="key"> 指定主键 </param>
        /// <returns> 符合编号的记录，不存在返回null </returns>
        TEntity SelectById(object key);

        /// <summary>
        /// 通过条件获取单条实体记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        TEntity GetByFilter(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

        /// <summary>
        /// 通过条件获取单条实体记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        TEntity GetByFilterWithTracking(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <returns> 操作影响的行数 </returns>
        int Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="funWhere">查询条件-谓语表达式</param>
        /// <param name="funUpdate">实体-谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        int Update(Expression<Func<TEntity, bool>> funWhere, Expression<Func<TEntity, TEntity>> funUpdate);

   
        /// <summary>
        /// 执行非查询sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="behavior">事务属性</param>
        /// <param name="isInsertIdentity">sql语句是否在自增字段插入值</param>
        /// <param name="tableName">操作的表名（可空）</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回影响的行数</returns>
        List<T> ExcuteQuery<T>(string sql, params object[] parameters);



        /// <summary>
        /// 指定使用索引
        /// </summary>
        /// <param name="indexName"></param>
        IQueryable<TEntity> UseIndex(string indexName);
        #endregion
    }
}
