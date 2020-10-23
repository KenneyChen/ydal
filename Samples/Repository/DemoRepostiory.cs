using EntityFramework;
using NetCore_Dal.Models;
using Repository.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public class DemoRepostiory : BaseEfRepository<Demo>,IDemoRepostiory
    {
        /// <summary>
        ///  获取 仓储上下文的实例
        /// </summary>
        private IUnitOfWork unitOfWork;

        public DemoRepostiory(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}
