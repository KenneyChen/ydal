using AspectCore.DynamicProxy;
using NetCore.Dal.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YDal.Repository;

namespace YDal.Interceptor
{
    public class YTransactionalAttribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            /* 
             * 方法①
             * 生命周期 scope Singleton 可以使用以下方法，未测试过，理论可行
               var ef = context.ServiceProvider.GetService(typeof(EFContenxt)) as EFContenxt;
            */

            //方法② 通过获取特性类对象，找到这个类型定义efcontext属性，如果没有就需要在service层加入属性兵器生成注入
            if (context.Implementation is IRepository)
            {
                var ef = (context.Implementation as IRepository).EfContext;
                using (var scope = ef.Database.BeginTransaction())
                {
                    await next(context);
                }
            }
        }
    }
}
