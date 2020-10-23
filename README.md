# y-dal
a niubi netcore orm   framework





> 项目框架介绍





## 使用步骤

>安装dll

```
Install-Package y.dal
```



> 注入代码

```c#
 public void ConfigureServices(IServiceCollection services)
 {
            services.AddControllers();

            services.AddDal();  //注入
 }           
```



> 生成框架代码

打开项目 CodeGenerator，配置相关参数运行

代码

①可以通过nuget下载  （推荐）

```powershell
Install-Package Y.Dal.CodeGenerator -Version 1.0.0
```



② git clone

https://github.com/KenneyChen/y-dal.git



```json
{
    "DbOption": {
        "ConnectionString": "server=localhost;uid=root;pwd=123456;port=3306;database=test;sslmode=Preferred;",
        "DbType": "MySQL", //SqlServer  mysql
        "Author": "作者名称",
        "OutputPath": "F:\\app\\y-dal\\cs", //生成代码路径
        "ModelsNamespace": "Samples.Models", //实体命名空间
        "IRepositoryNamespace": "Samples.Repository", //仓储接口命名空间
        "RepositoryNamespace": "Samples.Repository.Impl", //仓储命名空间
        "IServicesNamespace": "Samples.Services", //服务接口命名空间
        "ServicesNamespace": "Samples.Services.Impl", //服务命名空间
        "Tables":"" //指定表名生成，多个用逗号隔开，为空全部
    }
}

```



> 拷贝生成代码到你实际项目

代码输出目录："OutputPath": "F:\\app\\y-dal\\cs"   可自行调整



> 测试  



```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Samples.Models;
using Samples.Repository;

namespace Samples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IdemoRepository DemoRepostiory1;


        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IdemoRepository DemoRepostiory)
        {
            _logger = logger;
            this.DemoRepostiory1 = DemoRepostiory;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            DemoRepostiory1.Insert(new demo { UserName = "明哥富婆" });

            Console.WriteLine($"{DemoRepostiory1.GetHashCode()}");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

```

