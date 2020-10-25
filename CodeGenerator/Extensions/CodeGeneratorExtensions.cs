using Microsoft.Extensions.DependencyInjection;
using NetCore.Dal;
using NetCore.Dal.Helper;
using NetCore.Dal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace YDal.EntityFramework
{
    public static class CodeGeneratorExtensions
    {
        /// <summary>
        /// 代码生成器注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="ifExistCovered">文件存在是否覆盖，true-覆盖；否-不覆盖</param>
        public static void AddDalCodeGenerator(this IServiceCollection services,bool ifExistCovered = true)
        {
            var config = Common.GetConfiguration();
            if (string.IsNullOrWhiteSpace(config["DbOption:ConnectionString"]))
            {
                System.Diagnostics.Debug.WriteLine("警告：代码生成器DbOption.ConnectionString为空");
                return;
            }
            services.Configure<CodeGenerateOption>(config.GetSection("DbOption"));
            services.AddScoped<CodeGenerator>();

            services
                .BuildServiceProvider()
                .GetService<CodeGenerator>()
                .GenerateAllCodesFromDatabase(ifExistCovered);
        }
    }
}
