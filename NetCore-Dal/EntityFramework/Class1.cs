using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Dal.EntityFramework
{
    public interface IEntityTypeConfiguration
    {
        void Map(ModelBuilder builder);
    }

    public interface IEntityTypeConfiguration<T> : IEntityTypeConfiguration where T : class
    {
        void Map(EntityTypeBuilder<T> builder);
    }

    public abstract class WGEntityTypeConfig<T> : IEntityTypeConfiguration<T> where T : class
    {
        public abstract void Map(EntityTypeBuilder<T> builder);

        public void Map(ModelBuilder builder)
        {
            Map(builder.Entity<T>());
        }
    }

    public static class ModelBuilderExtentcs
    {
        private static IEnumerable<Type> GetMappingTypes(this Assembly assembly, Type mappingInterface)
        {
            return assembly.GetTypes().Where(x => !x.GetTypeInfo().IsAbstract && x.GetInterfaces().
            Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == mappingInterface));
        }

        public static void AddEntityConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly assembly)
        {
            var mappingTypes = assembly.GetTypes()
               .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(WGEntityTypeConfig<>));
            //var mappingTypes = assembly.GetMappingTypes(typeof(WGEntityTypeConfig<>));
            foreach (var config in mappingTypes.Select(Activator.CreateInstance).Cast<IEntityTypeConfiguration>())
            {
                config.Map(modelBuilder);
            }
        }
    }
}
