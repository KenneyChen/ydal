---
typora-copy-images-to: static
---

# y-dal


> 项目框架介绍

可能是东半球最好netcore ORM框架，目前暂时支持mysql、mssql，其核心设计目标是开发迅速、学习简单、轻量级、易扩展。现已开放源代码，开箱即用。

## 





## 使用步骤

>安装dll

```
Install-Package y.dal
Install-Package Y.Dal.CodeGenerator 
```

也可以使用nuget管理客户端进行安装

![https://github.com/KenneyChen/y-dal/blob/main/static/nuget.y.dal.png]()



> 注入代码

```c#
 public void ConfigureServices(IServiceCollection services)
 {
            services.AddControllers();

             services
               .AddDal()//类库核心功能
               .AddDalCodeGenerator();//代码生成器专用，生成框架代码之后请注释这段代码AddDalCodeGenerator; 
 }           
```



> 生成框架代码配置



```json
{
    //连接数据库专用
    "ConnectionStrings": {
        "DefaultConnection": "server=localhost;uid=root;pwd=123456;port=3306;database=test;sslmode=Preferred;"
    }
    
    //代码生成器插件使用 
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



## API使用

支持以下2种写法，根据自身需要选择其中一种即可，以下所有代码查看源码UnitTest项目

#### lambda

```c#
xxxRepository.Where(f=>f.Id==123);
```

**Linq**:

```c#
from a in xxxRepository
```



### 1、where （条件查询)

```c#
①简单使用
fitler
②多个条件
③多个表关联    
```



2、select（映射）



3、update （更新）



4、delete （删除）



5、insert （插入）



6、page分页 （分页）

