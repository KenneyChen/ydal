using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YDal.EntityFramework;

namespace UnitTest
{
    [TestClass]
    public class SelectTest1
    {
        public SelectTest1() 
        {
            var services = new ServiceCollection();
            services.AddDal();
            var provider = services.BuildServiceProvider();
            //provider.GetService
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
