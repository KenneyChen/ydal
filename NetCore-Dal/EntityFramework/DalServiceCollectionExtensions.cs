using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCore.Dal.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using YDal.Repository;
using YDal.UnitOfWork;
using YDal.UnitOfWork.Impl;

namespace YDal.EntityFramework
{
    public static class DalServiceCollectionExtensions
    {
        public static readonly ILoggerFactory efLogger = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => 
            category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
        });

        static void Debug(string info)
        {
            System.Diagnostics.Debug.WriteLine(info);
            Console.WriteLine(info);
        }

        public static IServiceCollection AddDal(this IServiceCollection services)
        {

            //设置工作目录，Linux下可能出现找不到目录
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            var builder = new ConfigurationBuilder();
            var config = builder.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connection = config.GetConnectionString("DefaultConnection");
            var dbType = config.GetConnectionString("DbType");
            if (string.IsNullOrEmpty(connection))
            {
                throw new Exception("请在appsettings.json配置数据库连接字符串 " +
                    "'ConnectionStrings':{'DefaultConnection':'xxxxx'} " +
                    "节点");
            }

            //注入dbcontext
            services.AddDbContext<DalDbContext>(
            options =>
            {
                switch (dbType?.ToLower())
                {
                    case "mssql":
                        options.UseSqlServer(connection);
                        break;
                    case "mysql":
                        options.UseMySQL(connection);
                        break;
                    default:
                        //默认是mysql
                        options.UseMySQL(connection);
                        break;
                }
                //增加sql日记记录
                options.UseLoggerFactory(efLogger);

            });

            //注入工作单元
            services.AddScoped<IUnitOfWork, EfUnitOfWorkContext>();

            //注入仓储
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                //IRepository
                {
                    var classTypes = item.GetTypes().Where(f => f.IsClass == true && typeof(IRepository).IsAssignableFrom(f)).ToList();
                    foreach (var cls in classTypes)
                    {
                        //后续扩展 通过cls特性 建立不同生命周期
                        var interfaceType = cls.GetInterfaces().FirstOrDefault(f => f.IsGenericType == false && !f.Equals(typeof(IRepository)));
                        if (interfaceType != null)
                        {
                            Debug($"接口{interfaceType},实现类：{cls},Scoped注入成功");
                            services.AddScoped(interfaceType, cls);
                        }
                    }
                }
            }
            foreach (var item in assemblies)
            {
                //IService
                {
                    var classTypes = item.GetTypes()
                        .Where(f => f.IsClass == true && f.FullName.ToLower().EndsWith("service"))
                        .ToList();
                    foreach (var cls in classTypes)
                    {
                        //后续扩展 通过cls特性 建立不同生命周期
                        //&& f.FullName.EndsWith("Service")
                        //NetCore.Dal.Services.IService`1
                        var interfaceType = cls
                            .GetInterfaces()
                            .FirstOrDefault(f => f.IsGenericType == false 
                            && f.GetInterfaces().Length==1 
                            && f.GetInterfaces().All(a=>a.Name==typeof(IBaseService<>).Name && a.Namespace== "NetCore.Dal.Services") 
                            && f.FullName.ToLower().EndsWith("service"));
                        if (interfaceType != null)
                        {
                            Debug($"接口{interfaceType},实现类：{cls},Scoped注入成功");
                            services.AddScoped(interfaceType, cls);
                        }
                    }
                }
            }

            return services;

        }


    }

    public static class IntrospectionExtensions
    {
        public static TypeInfo GetTypeInfo(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            IReflectableType type2 = (IReflectableType)type;
            return ((type2 != null) ? type2.GetTypeInfo() : null);
        }
    }
}
