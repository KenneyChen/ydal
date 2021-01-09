---
typora-copy-images-to: static
---

# ydal


> 项目框架介绍

可能是东半球最好netcore ORM框架，目前暂时支持mysql、mssql，其核心设计目标是开发迅速、学习简单、轻量级、易扩展。现已开放源代码，开箱即用。

It may be the best NETCORE ORM framework in the eastern hemisphere. At present, it temporarily supports MySQL and MSSQL, and its core design goal is to open

## 

> 特此声明

重复造轮子仅为netcore开源贡献自己一份力量，让用户在orm框架上多一份选择，可以免费商用并提供永久技术支持



## 使用步骤

>安装dll

```
Install-Package ydal
Install-Package YDal.CodeGenerator 
```

也可以使用nuget管理客户端进行安装

![avatar](https://github.com/KenneyChen/y-dal/blob/main/static/nuget.y.dal.png)



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
        "DefaultConnection": "server=localhost;uid=root;pwd=123456;port=3306;database=test;sslmode=Preferred;",
        "DbType": "MySQL"  //无此配置，默认mysql
    }
    //mssql连接字符串 Data Source=192.168.1.110;Initial Catalog=test;Persist Security Info=True;User ID=test;Password=123456;MultipleActiveResultSets=True;App=EntityFramework;
    //代码生成器插件使用 
    "DbOption": {
        "ConnectionString": "server=localhost;uid=root;pwd=123456;port=3306;database=test;sslmode=Preferred;",
        "DbType": "MySQL", //mssql  mysql
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
var order1 = _orderdetailRepository.Entities.Where(f => f.itemid == 123)
              .Select(f => new
              {
                  f.itemid
              })
              .FirstOrDefault();

var order1 = _orderdetailRepository.Filter(f => f.itemid == 123);

排序从小到大
var order1 = _orderdetailRepository.Filter(f => f.orderid == 1, p => p.OrderBy(f => f.Id));

排序从大到小
var order1 = _orderdetailRepository.Filter(f => f.orderid == 1, p => p.OrderByDescending(f => f.Id));

多个字段排序
var order1 = _orderdetailRepository.Filter(f => f.orderid == 1, p => p.OrderBy(f => f.Id).ThenBy(f=>f.itemid));    

②多个条件
var order1 = _orderdetailRepository.Filter(f => f.itemid == 123 && f.orderid == 1); 

③多个表关联 
var order1 = from a in _orderdetailRepository.Entities
             join b in _ordermasterRepository.Entities on a.orderid equals b.Id
             select new
             {
                 b.paytime,
                 a.totalprice,
                 a.itemid,
             };
```



### 2、select（映射）

```C#
①匿名对象
var order1 = _orderdetailRepository.Entities.Where(f => f.itemid == 123)
             .Select(f => new
              {
                  f.itemid
              })
             .FirstOrDefault();

②自定义对象
 public class Test
 {
        public string itemname04 { get; set; }
        public int itemid04 { get; set; }
 }
var order1 = _orderdetailRepository.Entities.Where(f => f.itemid == 123)
             .Select(f => new test
              {
                  f.itemid
              })
             .FirstOrDefault();
```

### 3、update （更新）

```C#
①简单更新（不推荐）
var order1 = _orderdetailRepository.FilterWithTracking(f => f.itemid == 123);
order1.totalprice = 30.01M;
var r= _orderdetailRepository.Update(order1);

②sql语法糖更新（强烈推荐）
 var r=_orderdetailRepository.Update(f => f.itemid == 123, f => new Orderdetail
         {
            totalprice = 32.01M
         });    
```

### 4、delete （删除）

```C#
① 简单删除（不推荐）
var order1 = _orderdetailRepository.FilterWithTracking(f => f.itemid == 9990);
var r = _orderdetailRepository.Delete(order1);

② sql语法糖更新（强烈推荐）
var r = _orderdetailRepository.Delete(f => f.itemid == 9990);
```

### 5、insert （插入）

```C#
①、简单插入
var r = _orderdetailRepository.Insert(new Orderdetail
            {
                itemid = 111115,
                itemname = "测试insert",
                totalprice = 111,
                orderid = 1,
            });
            
② 批量插入
var itemId = new Random().Next(100000, 999999);
var list = new List<Orderdetail>();
for (int i = 0; i < 10; i++)
{
    list.Add(new Orderdetail
    {
       itemid = itemId,
       itemname = "测试插入",
       totalprice = 111,
       orderid = 1,
       });
    };
var r = _orderdetailRepository.Insert(list);
```

### 6、page分页 （分页）

```C#
①无条件单表分页
var page = new PagingInfo
{
     PageIndex = 2, //当前页
     PageSize = 5,//每页条数
};
var result = _orderdetailRepository.GetListByPage(page);
var totalCount = result.TotalCount;//返回总页数 

② 有条件单表分页
var page = new PagingInfo
{
    PageIndex = 2, //当前页
    PageSize = 5,//每页条数
};
var result = _orderdetailRepository.Entities.Where(f => f.Id > 10).Paging(page); 

③ 支持匿名对象分页
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

④ 支持自定义对象分页
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


⑤多表分页
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
```



> 问题反馈

可以直接进群反馈：916213430