using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSql2Table.Models;
using NSql2Table.Repository;
using NSql2Table.Services;
using System;
using System.Linq;
using YDal.EntityFramework;

namespace UnitTest
{
    [TestClass]
    public class DeleteTest1
    {
        private readonly IOrderdetailRepository _orderdetailRepository;

        private readonly IOrdermasterRepository _ordermasterRepository;

        public DeleteTest1()
        {
            var services = new ServiceCollection();
            services.AddDal();//.AddDalCodeGenerator();
            var provider = services.BuildServiceProvider();
            _orderdetailRepository = provider.GetService<IOrderdetailRepository>();

            _ordermasterRepository = provider.GetService<IOrdermasterRepository>();
        }



        [TestMethod]
        public void TestDeleteSample()
        {
            _orderdetailRepository.Insert(new Orderdetail
            {
                itemid = 9990,
                itemname = "²âÊÔÉ¾³ý",
                totalprice = 111,
                orderid = 1,
            });

            var order1 = _orderdetailRepository.FilterWithTracking(f => f.itemid == 9990);
            var r = _orderdetailRepository.Delete(order1);
            Assert.IsTrue(r > 0);
            var o = _orderdetailRepository.Filter(f => f.itemid == 9990);
            Assert.IsTrue(o==null);
        }

        [TestMethod]
        public void TestDelete()
        {
            _orderdetailRepository.Insert(new Orderdetail
            {
                itemid = 9990,
                itemname = "²âÊÔÉ¾³ý",
                totalprice = 111,
                orderid = 1,
            });

            var r = _orderdetailRepository.Delete(f => f.itemid == 9990);

            Assert.IsTrue(r > 0);
            var o = _orderdetailRepository.Filter(f => f.itemid == 9990);
            Assert.IsTrue(o==null);
        }
    }
}
