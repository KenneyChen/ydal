using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSql2Table.Models;
using NSql2Table.Repository;
using NSql2Table.Services;
using System.Linq;
using YDal.EntityFramework;
using YDal;
using NetCore.Dal.Models;

namespace UnitTest
{
    [TestClass]
    public class SelectTest1
    {
        private readonly IOrderdetailRepository _orderdetailRepository;

        private readonly IOrdermasterRepository _ordermasterRepository;

        public SelectTest1()
        {
            var services = new ServiceCollection();
            services.AddDal();//.AddDalCodeGenerator();
            var provider = services.BuildServiceProvider();
            _orderdetailRepository = provider.GetService<IOrderdetailRepository>();

            _ordermasterRepository = provider.GetService<IOrdermasterRepository>();
        }

        [TestMethod]
        public void TestSelect()
        {
            try
            {
                var order1 = _orderdetailRepository.Entities.Where(f => f.itemid == 123)
              .Select(f => new
              {
                  f.itemid
              })
              .FirstOrDefault();


                Assert.IsTrue(order1 != null && order1.itemid == 123);
            }
            catch (System.Exception e)
            {

                throw;
            }
        }

        [TestMethod]
        public void TestFilter()
        {
            var order1 = _orderdetailRepository.Filter(f => f.itemid == 123);
            Assert.IsTrue(order1 != null && order1.itemid == 123);
        }

        [TestMethod]
        public void TestFilter2()
        {
            var order1 = _orderdetailRepository.Filter(f => f.itemid == 123 && f.orderid == 1);
            Assert.IsTrue(order1 != null && order1.itemid == 123);
        }

        [TestMethod]
        public void TestFilterOrderBy()
        {
            var order1 = _orderdetailRepository.Filter(f => f.orderid == 1, p => p.OrderBy(f => f.Id));
            Assert.IsTrue(order1 != null && order1.itemid == 123);
        }

        [TestMethod]
        public void TestFilterOrderByDescending()
        {
            var order1 = _orderdetailRepository.Filter(f => f.orderid == 1, p => p.OrderByDescending(f => f.Id));

            var maxId = _orderdetailRepository.Entities
                .Where(f => f.orderid == 1)
                .Max(f => f.Id);

            var order2 = _orderdetailRepository.Filter(f => f.Id == maxId);
            Assert.IsTrue(order2.Id==order1.Id);
        }

        [TestMethod]
        public void TestJoin()
        {
            var order1 = from a in _orderdetailRepository.Entities
                         join b in _ordermasterRepository.Entities on a.orderid equals b.Id
                         select new
                         {
                             b.paytime,
                             a.totalprice,
                             a.itemid,
                         };
            Assert.IsTrue(order1.FirstOrDefault() != null);
        }

        /// <summary>
        /// 单表分页01
        /// </summary>
        [TestMethod]
        public void TestPageTableOne01()
        {
            var page = new PagingInfo
            {
                PageIndex = 2, //当前页
                PageSize = 5,//每页条数
            };
            var result = _orderdetailRepository.GetListByPage(page);
            var total = _orderdetailRepository.Entities.Count();
            Assert.IsTrue(result.Records.Count == 5 && result.TotalCount == total);
        }

        /// <summary>
        /// 单表分页02(附加条件）
        /// </summary>
        [TestMethod]
        public void TestPageTableOne02()
        {
            var page = new PagingInfo
            {
                PageIndex = 2, //当前页
                PageSize = 5,//每页条数
            };
            var result = _orderdetailRepository.Entities.Where(f => f.Id > 10).Paging(page);
            var total = _orderdetailRepository.Entities.Where(f => f.Id > 10).Count();
            Assert.IsTrue(result.Records.Count == 5 && result.TotalCount == total);
        }

        /// <summary>
        /// 单表分页03(匿名）
        /// </summary>
        [TestMethod]
        public void TestPageTableOne03()
        {
            var page = new PagingInfo
            {
                PageIndex = 1, //当前页
                PageSize = 5,//每页条数
            };
            var result = _orderdetailRepository.Entities
                .Where(f => f.Id > 10)
                .Select(p => new
                {
                    p.itemid,
                    p.itemname
                })
                .Paging(page);
            var total = _orderdetailRepository.Entities.Where(f => f.Id > 10).Count();

            Assert.IsTrue(result.Records.Count == 5 && result.TotalCount == total);
        }


        /// <summary>
        /// 单表分页03(自定义实体）
        /// </summary>
        [TestMethod]
        public void TestPageTableOne04()
        {
            var page = new PagingInfo
            {
                PageIndex = 1, //当前页
                PageSize = 5,//每页条数
            };
            var result = _orderdetailRepository.Entities
                .Where(f => f.Id > 10)
                .Select(p => new Test
                {
                    itemid04 = p.itemid,
                    itemname04 = p.itemname
                })
                .Paging(page);
            var total = _orderdetailRepository.Entities.Where(f => f.Id > 10).Count();

            Assert.IsTrue(result.Records.Count == 5 && result.TotalCount == total);
        }

        /// <summary>
        /// 多表分页01
        /// </summary>
        [TestMethod]
        public void TestPageTableJoinMany01()
        {
            var page = new PagingInfo
            {
                PageIndex = 2, //当前页
                PageSize = 5,//每页条数
            };
            var query = from a in _orderdetailRepository.Entities
                         join b in _ordermasterRepository.Entities on a.orderid equals b.Id
                         select new
                         {
                             b.paytime,
                             a.totalprice,
                             a.itemid,
                         };
            var result = query.Paging(page);

            var total = query.Count();
            Assert.IsTrue(result.Records.Count == 5 && result.TotalCount == total);
        }

    }
    public class Test
    {
        public string itemname04 { get; set; }
        public int itemid04 { get; set; }
    }
}
