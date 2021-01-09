using Microsoft.EntityFrameworkCore;
using NetCore.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YDal.Common;
using YDal.EntityFramework;
using YDal.UnitOfWork;
using YDal.UnitOfWork.Impl;

namespace YDal.Repository.Impl
{

    /// <summary>
    /// EntityFramework仓储操作基类
    /// </summary>
    /// <typeparam name="TEntity">动态实体类型</typeparam>
    public abstract class BaseEfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        #region 公共方法


        private readonly DalDbContext _efDbContext;

        public BaseEfRepository(DalDbContext efDbContext)
        {
            this._efDbContext = efDbContext;
        }


        public DalDbContext EfContext
        {
            get { return _efDbContext; }
        }

        public IQueryable<TEntity> Entities => EfContext.Set<TEntity>().AsNoTracking();

        public IQueryable<TEntity> Table => EfContext.Set<TEntity>();


        /// <summary>
        /// 插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual void Insert(TEntity entity)
        {
            PublicHelper.CheckArgument(entity, "entity");
            EfContext.RegisterNew(entity);
        }

        /// <summary>
        ///     批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            PublicHelper.CheckArgument(entities, "entities");
            EfContext.RegisterNew(entities);

        }



        /// <summary>
        ///     删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual void Delete(TEntity entity)
        {
            PublicHelper.CheckArgument(entity, "entity");
            EfContext.RegisterDeleted<TEntity>(entity);
        }

        /// <summary>
        ///     删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            PublicHelper.CheckArgument(entities, "entities");
            EfContext.RegisterDeleted<TEntity>(entities);
        }


        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            PublicHelper.CheckArgument(predicate, "predicate");
            List<TEntity> entities = EfContext.Set<TEntity>().Where(predicate).ToList();
            if (entities.Count > 0)
            {
                Delete(entities);
            }
        }

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual void Update(TEntity entity)
        {
            PublicHelper.CheckArgument(entity, "entity");
            EfContext.RegisterModified<TEntity>(entity);
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            PublicHelper.CheckArgument(entities, "entities");
            EfContext.RegisterModified<TEntity>(entities);
        }


        /// <summary>
        /// 查找指定主键的实体记录
        /// </summary>
        /// <param name="key"> 指定主键 </param>
        /// <returns> 符合编号的记录，不存在返回null </returns>
        public virtual TEntity SelectById(object key)
        {
            PublicHelper.CheckArgument(key, "key");
            return EfContext.Set<TEntity>().Find(key);
        }

        /// <summary>
        /// 根据条件获取单条实体数据 (无实体跟踪)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TEntity Filter(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            var query = EfContext.Set<TEntity>().AsNoTracking().Where(filter);
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            HandleInclude(ref query, includeProperties);
            return query.FirstOrDefault();
        }

        public TEntity FilterWithTracking(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            var query = EfContext.Set<TEntity>().Where(filter);
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            HandleInclude(ref query, includeProperties);
            return query.FirstOrDefault();
        }

        /// <summary>
        /// 删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int DeleteFunc(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate).DeleteFromQuery();
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="funWhere">查询条件-谓语表达式</param>
        /// <param name="funUpdate">实体-谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Update(Expression<Func<TEntity, bool>> funWhere, Expression<Func<TEntity, TEntity>> funUpdate)
        {
            return Entities.Where(funWhere).UpdateFromQuery(funUpdate);
        }

        /// <summary>
        /// 单表默认分页功能
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public Page<TEntity> GetListByPage(PagingInfo pageInfo)
        {
            return GetListByPage(pageInfo, null, null, "");
        }


        /// <summary>
        /// 多表分页查询
        /// </summary>
        /// <param name="query">IQueryable对象</param>
        /// <returns></returns>
        public Page<T> GetListByPage<T>(PagingInfo pageInfo, IQueryable<T> query)
        {
            return query.Paging(pageInfo);
        }

        /// <summary>
        /// 单表多个条件分页查询
        /// </summary>
        /// <param name="filter">where条件</param>
        /// <param name="orderBy">分页信息，会返回查询后的总条数（字段RecCount）</param>
        /// <param name="pageInfo">分页信息，字段RecCount返回总条数，可以不分页，NeedPage=false即可不分页</param>
        /// <param name="includeProperties">include的关联实体s，多个用逗号隔开</param>
        /// <returns></returns>
        public Page<TEntity> GetListByPage(PagingInfo pageInfo, Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties = "")
        {
            var page = new Page<TEntity>();

            //纠正分页参数
            pageInfo = pageInfo == null ? new PagingInfo { PageIndex = 1, PageSize = 10 } : pageInfo;
            pageInfo.PageSize = pageInfo.PageSize > 0 ? pageInfo.PageSize : 10;
            IQueryable<TEntity> query = filter != null ? Entities.Where(filter) : Entities;
            HandleInclude(ref query, includeProperties);
            page.TotalCount = query.Count();
            if (page.TotalCount == 0 && pageInfo.NeedPage)
            {
                return page;
            }
            query = orderBy != null ? orderBy(query) : query;
            if (pageInfo.NeedPage)
            {
                var queryPage = query
                    .Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize)
                    .Take(pageInfo.PageSize);

                page.Records = queryPage.ToList();
                return page;
            }
            else
            {
                page.Records = query.ToList();
                return page;
            }
        }

        /// <summary>
        /// 执行非查询sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="behavior">事务属性</param>
        /// <param name="isInsertIdentity">sql语句是否在自增字段插入值</param>
        /// <param name="tableName">操作的表名（可空）</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回影响的行数</returns>
        public virtual int ExcuteNoQuery(string sql, bool isInsertIdentity = false, string tableName = "", params object[] parameters)
        {
            if (isInsertIdentity)
            {
                if (tableName == "") tableName = typeof(TEntity).Name;
                if (sql.TrimEnd().EndsWith(";") == false) sql = sql + ";";
                sql = "SET IDENTITY_INSERT " + tableName + " ON;" + sql + "SET IDENTITY_INSERT " + tableName + "  OFF;";
            }
            return EfContext.Database.ExecuteSqlRaw(sql, parameters);
        }
        /// <summary>
        /// 执行查询sql语句
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public virtual List<T> ExcuteQuery<T>(string sql, params object[] parameters)
        {
            return EfContext.Database.SqlQueryWithNoLock<T>(sql, parameters).ToList();
        }
        #endregion


        private void HandleInclude(ref IQueryable<TEntity> query, string includeProperties)
        {
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                   (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
        }

        public void RemoveFromContext(TEntity entity)
        {
            PublicHelper.CheckArgument(entity, "entity");
            EfContext.RegisterRemove(entity);
        }


        public IQueryable<TEntity> UseIndex(string indexName)
        {
            throw new Exception("test");
            //    var type = typeof(TEntity);
            //    var tableAttr = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>();
            //    var tableName = tableAttr == null ? type.Name : tableAttr.Name;
            //    var efDbContext = EfContext.DbContext as EfDbContext;
            //    if (efDbContext != null)
            //    {
            //        if (efDbContext.AssignTableIndexs == null)
            //            efDbContext.AssignTableIndexs = new Dictionary<string, string>();

            //        if (!efDbContext.AssignTableIndexs.ContainsKey(tableName))
            //            efDbContext.AssignTableIndexs.Add(tableName, indexName);
            //        else
            //            efDbContext.AssignTableIndexs[tableName] = indexName;
            //    }

            //    return this.Entities;
        }

    }

}
