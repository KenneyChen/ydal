using Microsoft.EntityFrameworkCore;
using NetCore.Dal.Models;
using NetCore.Dal.Models;
using System;
using System.Linq;
using System.Reflection;

namespace EntityFramework
{
    public class DalDbContext : DbContext
    {
        //public DbSet<Demo> Demo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public DalDbContext(DbContextOptions<DalDbContext> options) : base(options)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                foreach (var type in item.GetTypes())
                {
                    if (type.IsClass && type != typeof(BaseEntity) && typeof(BaseEntity).IsAssignableFrom(type))
                    {
                        modelBuilder.Entity(type);
                        ////var method = modelBuilder.GetType().GetMethods().Where(x => x.Name == "Entity").FirstOrDefault();
                        ////if (method != null)
                        ////{
                        ////    method = method.MakeGenericMethod(new Type[] { type });
                        ////    method.Invoke(modelBuilder, null);
                        ////}
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
