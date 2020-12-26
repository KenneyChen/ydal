using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YDal.EntityFramework;

namespace Samples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDal().AddDalCodeGenerator();
            ////×¢²á·þÎñ
            //services.AddDbContext<DalDbContext>(
            //        options =>
            //        {
            //           options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //        }
            //    );

            //services.AddScoped<IUnitOfWork, EfUnitOfWorkContext>();

            ////var r= typeof(IRepository<>);
            ////services.AddScoped<IRepository<>, DemoRepostiory>();


            //var r=services.BuildServiceProvider().GetService(typeof(IRepository));


            //var r2 = services.BuildServiceProvider().GetService(typeof(IDemoRepostiory));
            //Console.WriteLine($"{r2.GetHashCode()}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();


            //var r1=app.ApplicationServices.GetService(typeof(IDemoRepostiory));
            //Console.WriteLine($"{r1.GetHashCode()}");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
