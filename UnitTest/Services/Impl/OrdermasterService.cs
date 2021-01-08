/**
*┌──────────────────────────────────────────────────────────────┐
*│　描    述：{Comment}                                                    
*│　作    者：{Author}                                            
*│　版    本：1.0    模板代码自动生成                                                
*│　创建时间：{GeneratorTime}                             
*└──────────────────────────────────────────────────────────────┘
*┌──────────────────────────────────────────────────────────────┐
*│　命名空间： NSql2Table.Services.Impl                                  
*│　类    名： OrdermasterService                                    
*└──────────────────────────────────────────────────────────────┘
*/
using NetCore.Dal.Services.Impl;
using NSql2Table.Models;
using NSql2Table.Repository;
using NSql2Table.Services;
using System;
using System.Linq;
using NetCore.Dal.Services;

namespace NSql2Table.Services.Impl
{
    public class OrdermasterService: BaseService<IOrdermasterRepository, Ordermaster>,
        IOrdermasterService
    {
        private readonly IOrdermasterRepository _repository;

        public OrdermasterService(IOrdermasterRepository repository) : base(repository)
        {
            this._repository = repository;
        }
    }
}