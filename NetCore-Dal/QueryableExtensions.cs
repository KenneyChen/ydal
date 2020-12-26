using NetCore.Dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace YDal
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// 多表分页查询(也支持单表，但是建议使用GetListByPage)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public static Page<T> Paging<T>(this IQueryable<T> query, PagingInfo pageInfo)
        {
            var page = new Page<T>();
            //纠正分页参数
            pageInfo = pageInfo == null ? new PagingInfo { PageIndex = 1, PageSize = 10 } : pageInfo;
            pageInfo.PageSize = pageInfo.PageSize > 0 ? pageInfo.PageSize : 10;
            page.TotalCount = query.Count();
            if (page.TotalCount == 0 && pageInfo.NeedPage)
            {
                return page;
            }

            page.Records = query.ToList();
            return page;
        }
    }
}
