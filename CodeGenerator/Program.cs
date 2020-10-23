using Microsoft.Extensions.DependencyInjection;
using NetCore.Dal;
using NetCore.Dal.Helper;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = Common.BuildServiceForSqlServer();
            var codeGenerator = serviceProvider.GetService<CodeGenerator>();
            codeGenerator.GenerateAllCodesFromDatabase(true);
            Console.WriteLine("生成成功!");
            Console.ReadLine();
        }
    }
}
