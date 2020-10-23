using EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Dal.UnitOfWork.Impl
{
    public class EfUnitOfWorkContext : BaseUnitOfWorkContext
    {

        private readonly DalDbContext _efDbContext;


        public EfUnitOfWorkContext(DalDbContext efDbContext)
        {
            this._efDbContext = efDbContext;
        }

        /// <summary>
        /// 获取 当前使用的数据访问上下文对象
        /// </summary>
        protected override DbContext Context
        {
            get
            {
                return _efDbContext;
            }
        }

    }
}
