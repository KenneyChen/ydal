using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSql2Table.Models;
using NSql2Table.Repository;
using NSql2Table.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using YDal.EntityFramework;

namespace UnitTest
{
    [TestClass]
    public class InsertTest1
    {
        private readonly IOrderdetailRepository _orderdetailRepository;

        private readonly IOrdermasterRepository _ordermasterRepository;

        public InsertTest1()
        {
            var services = new ServiceCollection();
            services.AddDal();//.AddDalCodeGenerator();
            var provider = services.BuildServiceProvider();
            _orderdetailRepository = provider.GetService<IOrderdetailRepository>();

            _ordermasterRepository = provider.GetService<IOrdermasterRepository>();
        }



        [TestMethod]
        public void TestInsertSample()
        {
            var r = _orderdetailRepository.Insert(new Orderdetail
            {
                itemid = 111115,
                itemname = "²âÊÔinsert",
                totalprice = 111,
                orderid = 1,
            });

            //Assert.IsTrue(r > 0);

            var o= _orderdetailRepository.Filter(f => f.itemid == 111115);

            Assert.IsTrue(o != null);
        }

        [TestMethod]
        public void TestInsertBatch()
        {
            var itemId = new Random().Next(100000, 999999);
            var list = new List<Orderdetail>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new Orderdetail
                {
                    itemid = itemId,
                    itemname = "²âÊÔ²åÈë",
                    totalprice = 111,
                    orderid = 1,
                    
                });
            };
            var r = _orderdetailRepository.Insert(list);

            //Assert.IsTrue(r > 0);

            var o = _orderdetailRepository.Entities
                .Where(f => f.itemid == itemId)
                .ToList();

            Assert.IsTrue(o.Count==10);
        }
    }
}
