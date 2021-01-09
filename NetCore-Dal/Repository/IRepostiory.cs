using Microsoft.EntityFrameworkCore;
using NetCore.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YDal.EntityFramework;

namespace YDal.Repository
{
    public interface IRepository
    {
        DalDbContext EfContext { get; }
    }

    public interface IRepository<TEntity> : IRepository
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
        void Insert(TEntity entity);

        /// <summary>
        ///     批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        void Insert(IEnumerable<TEntity> entities);


        /// <summary>
        ///  删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        void Delete(TEntity entity);

        /// <summary>
        ///     删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <returns> 操作影响的行数 </returns>
        int DeleteFunc(Expression<Func<TEntity, bool>> predicate);


        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        void Update(TEntity entity);

        void Update(IEnumerable<TEntity> entities);


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
        TEntity Filter(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

        /// <summary>
        /// 通过条件获取单条实体记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        TEntity FilterWithTracking(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");


        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="funWhere">查询条件-谓语表达式</param>
        /// <param name="funUpdate">实体-谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        int Update(Expression<Func<TEntity, bool>> funWhere, Expression<Func<TEntity, TEntity>> funUpdate);


        /// <summary>
        /// 单表默认分页功能
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        Page<TEntity> GetListByPage(PagingInfo pageInfo);

        /// <summary>
        /// 多表分页查询
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <returns></returns>
        Page<T> GetListByPage<T>(PagingInfo pageInfo, IQueryable<T> query);

        /// <summary>
        /// 单表多个条件分页查询
        /// </summary>
        /// <param name="filter">where条件</param>
        /// <param name="orderBy">分页信息，会返回查询后的总条数（字段RecCount）</param>
        /// <param name="pageInfo">分页信息，字段RecCount返回总条数，可以不分页，NeedPage=false即可不分页</param>
        /// <param name="includeProperties">include的关联实体s，多个用逗号隔开</param>
        /// <returns></returns>
        Page<TEntity> GetListByPage(PagingInfo pageInfo, Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties = "");

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
