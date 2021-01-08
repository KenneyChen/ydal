using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSql2Table.Models;
using NSql2Table.Repository;
using NSql2Table.Services;
using System.Linq;
using YDal.EntityFramework;

namespace UnitTest
{
    [TestClass]
    public class UpdateTest1
    {
        private readonly IOrderdetailRepository _orderdetailRepository;

        private readonly IOrdermasterRepository _ordermasterRepository;

        public UpdateTest1()
        {
            var services = new ServiceCollection();
            services.AddDal();//.AddDalCodeGenerator();
            var provider = services.BuildServiceProvider();
            _orderdetailRepository = provider.GetService<IOrderdetailRepository>();

            _ordermasterRepository = provider.GetService<IOrdermasterRepository>();
        }



        [TestMethod]
        public void TestUpdateSample()
        {
            var order1 = _orderdetailRepository.FilterWithTracking(f => f.itemid == 123);
            order1.totalprice = 30.01M;
            var r= _orderdetailRepository.Update(order1);
            Assert.IsTrue(r > 0);
            var o=_orderdetailRepository.Filter(f => f.itemid == 123);
            Assert.IsTrue(o.totalprice == 30.01M);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var r=_orderdetailRepository.Update(f => f.itemid == 123, f => new Orderdetail
            {
                totalprice = 32.01M
            });

            Assert.IsTrue(r > 0);
            var o = _orderdetailRepository.Filter(f => f.itemid == 123);
            Assert.IsTrue(o.totalprice == 32.01M);
        }
    }
}
