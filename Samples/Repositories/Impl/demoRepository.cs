/**
*┌──────────────────────────────────────────────────────────────┐
*│　描    述：{Comment}接口实现                                                    
*│　作    者：{Author}                                            
*│　版    本：1.0    模板代码自动生成                                                
*│　创建时间：{GeneratorTime}                             
*└──────────────────────────────────────────────────────────────┘
*┌──────────────────────────────────────────────────────────────┐
*│　命名空间： {RepositoryNamespace}                                  
*│　类    名： {ModelName}Repository                                      
*└──────────────────────────────────────────────────────────────┘
*/
using Samples.Models;
using Samples.Repository;
using System;
using System.Threading.Tasks;
using YDal.Repository;
using YDal.Repository.Impl;
using YDal.UnitOfWork;

namespace Samples.Repository.Impl
{
    public class demoRepository: BaseEfRepository<demo>,IdemoRepository
    {
        /// <summary>
        ///  获取 仓储上下文的实例
        /// </summary>
        private IUnitOfWork unitOfWork;

        public demoRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}