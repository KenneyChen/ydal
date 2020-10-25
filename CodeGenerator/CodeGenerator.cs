using Microsoft.Extensions.Options;
using NetCore.Dal.Extensions;
using NetCore.Dal.Helper;
using NetCore.Dal.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCore.Dal
{
    /// <summary>
    /// 代码生成器。
    /// <remarks>
    /// 根据指定的实体域名空间生成Repositories和Services层的基础代码文件。
    /// </remarks>
    /// </summary>
    public class CodeGenerator
    {
        private string Delimiter = "\\";//分隔符，默认为windows下的\\分隔符
        private CodeGenerateOption Option;

        /// <summary>
        /// 静态构造函数：从IoC容器读取配置参数，如果读取失败则会抛出ArgumentNullException异常
        /// </summary>
        public CodeGenerator(IOptions<CodeGenerateOption> option)
        {
            this.Option = option.Value;
            //Options = ServiceLocator.Resolve<IOptions<CodeGenerateOption>>().Value;
            //if (Options == null)
            //{
            //    throw new ArgumentNullException(nameof(Options));
            //}
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var flag = path.IndexOf("/bin");
            if (flag > 0)
                Delimiter = "/";//如果可以取到值，修改分割符

            var arr = path.Split("bin");
            if (string.IsNullOrWhiteSpace(Option.ModelOutputPath))
            {
                Option.ModelOutputPath = arr[0];
            }
            if (string.IsNullOrWhiteSpace(Option.RepositoryOutputPath))
            {
                Option.RepositoryOutputPath = arr[0];
            }
        }


        /// <summary>
        /// 从代码模板中读取内容
        /// </summary>
        /// <param name="templateName">模板名称，应包括文件扩展名称。比如：template.txt</param>
        /// <returns></returns>
        public string ReadTemplate(string templateName)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var content = string.Empty;
            using (var stream = currentAssembly.GetManifestResourceStream($"{currentAssembly.GetName().Name}.CodeTemplate.{templateName}"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }
            return content;
        }

        /// <summary>
        /// 生成IRepository层代码文件
        /// </summary>
        /// <param name="modelTypeName"></param>
        /// <param name="keyTypeName"></param>
        /// <param name="ifExistCovered"></param>
        public void GenerateIRepository(string modelTypeName, bool ifExistCovered = false)
        {
            var iRepositoryPath = Option.RepositoryOutputPath + Delimiter + "Repositories";
            if (!Directory.Exists(iRepositoryPath))
            {
                Directory.CreateDirectory(iRepositoryPath);
            }
            var fullPath = iRepositoryPath + Delimiter + "I" + modelTypeName + "Repository.cs";
            if (File.Exists(fullPath) && !ifExistCovered)
                return;
            var content = ReadTemplate("IRepository.txt");
            content = content.Replace("{ModelsNamespace}", Option.ModelsNamespace)
                .Replace("{IRepositoriesNamespace}", Option.IRepositoryNamespace)
                .Replace("{ModelTypeName}", modelTypeName)
                .Replace("{GeneratorTime}", Option.GeneratorTime)
                .Replace("{Author}", Option.Author)
                .Replace("{KeyTypeName}", "");
            WriteAndSave(fullPath, content);
        }

        /// <summary>
        /// 生成Repository层代码文件
        /// </summary>
        /// <param name="modelTypeName"></param>
        /// <param name="keyTypeName"></param>
        /// <param name="ifExistCovered"></param>
        public void GenerateRepository(string modelTypeName, bool ifExistCovered = false)
        {
            var repositoryPath = Option.RepositoryOutputPath + Delimiter + "Repositories/Impl";
            if (!Directory.Exists(repositoryPath))
            {
                Directory.CreateDirectory(repositoryPath);
            }
            var fullPath = repositoryPath + Delimiter + modelTypeName + "Repository.cs";
            if (File.Exists(fullPath) && !ifExistCovered)
                return;
            var content = ReadTemplate("Repository.txt");
            content = content.Replace("{ModelsNamespace}", Option.ModelsNamespace)
                .Replace("{IRepositoriesNamespace}", Option.IRepositoryNamespace)
                .Replace("{RepositoriesNamespace}", Option.RepositoryNamespace)
                .Replace("{ModelTypeName}", modelTypeName)
                .Replace("{GeneratorTime}", Option.GeneratorTime)
                .Replace("{Author}", Option.Author)
                .Replace("{KeyTypeName}", "");
            WriteAndSave(fullPath, content);
        }

        ///// <summary>
        ///// 生成IRepository层代码文件
        ///// </summary>
        ///// <param name="modelTypeName"></param>
        ///// <param name="keyTypeName"></param>
        ///// <param name="ifExistCovered"></param>
        //public void GenerateIService(string modelTypeName, bool ifExistCovered = false)
        //{
        //    var iRepositoryPath = Option.OutputPath + Delimiter + "IServices";
        //    if (!Directory.Exists(iRepositoryPath))
        //    {
        //        Directory.CreateDirectory(iRepositoryPath);
        //    }
        //    var fullPath = iRepositoryPath + Delimiter + "I" + modelTypeName + "Service.cs";
        //    if (File.Exists(fullPath) && !ifExistCovered)
        //        return;
        //    var content = ReadTemplate("IService.txt");
        //    content = content.Replace("{ModelsNamespace}", Option.ModelsNamespace)
        //        .Replace("{IRepositoriesNamespace}", Option.IRepositoryNamespace)
        //        .Replace("{IServicesNamespace}", Option.IServicesNamespace)
        //        .Replace("{ModelTypeName}", modelTypeName)
        //        .Replace("{KeyTypeName}", "");
        //    WriteAndSave(fullPath, content);
        //}

        ///// <summary>
        ///// 生成Repository层代码文件
        ///// </summary>
        ///// <param name="modelTypeName"></param>
        ///// <param name="keyTypeName"></param>
        ///// <param name="ifExistCovered"></param>
        //public void GenerateService(string modelTypeName, bool ifExistCovered = false)
        //{
        //    var repositoryPath = Option.OutputPath + Delimiter + "Services";
        //    if (!Directory.Exists(repositoryPath))
        //    {
        //        Directory.CreateDirectory(repositoryPath);
        //    }
        //    var fullPath = repositoryPath + Delimiter + modelTypeName + "Service.cs";
        //    if (File.Exists(fullPath) && !ifExistCovered)
        //        return;
        //    var content = ReadTemplate("Service.txt");
        //    content = content.Replace("{ModelsNamespace}", Option.ModelsNamespace)
        //        .Replace("{IRepositoriesNamespace}", Option.IRepositoryNamespace)
        //        .Replace("{IServicesNamespace}", Option.IServicesNamespace)
        //        .Replace("{ServicesNamespace}", Option.ServicesNamespace)
        //        .Replace("{ModelTypeName}", modelTypeName)
        //        .Replace("{KeyTypeName}", "");
        //    WriteAndSave(fullPath, content);
        //}

        /// <summary>
        /// 根据数据表生成Model层、Controller层、IRepository层和Repository层代码
        /// </summary>
        /// <param name="ifExistCovered">是否覆盖已存在的同名文件</param>
        public void GenerateAllCodesFromDatabase(bool ifExistCovered = false)
        {
            var dbType = ConnectionFactory.GetDataBaseType(Option.DbType);

            using (var dbConnection = dbType.CreateConnection(Option.ConnectionString))
            {
                //获取所有表和所有列表
                var tables = dbConnection.GetCurrentDatabaseTableList(dbType);
                foreach (var table in tables)
                {
                    if (!string.IsNullOrWhiteSpace(Option.Tables))
                    {
                        var mytables = Option.Tables.Split(',').ToList();
                        if (!mytables.Any(f => f.Equals(table.TableName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            //不是用户指定表 过滤
                            continue;
                        }
                    }

                    //生成实体
                    GenerateEntity(table);

                    //生成仓储
                    GenerateIRepository(table.TableName, ifExistCovered);

                    //生成仓储
                    GenerateRepository(table.TableName, ifExistCovered);

                    //生成service
                    //GenerateIService(table.TableName, ifExistCovered);

                    //生成service
                    //GenerateService(table.TableName, ifExistCovered);
                }
            }


        }

        public void SetDefaultRootPath(CodeGenerateOption option)
        {
            var path = AppContext.BaseDirectory;
            var arr = path.Split(@"\bin\");
            if (!string.IsNullOrWhiteSpace(Option.ModelOutputPath))
            {
                Option.ModelOutputPath = arr[0];
            }
            if (!string.IsNullOrWhiteSpace(Option.RepositoryOutputPath))
            {
                Option.RepositoryOutputPath = arr[0];
            }
        }

        public void GenerateEntity(DbTable table, bool ifExistCovered = false)
        {
            //F:\\Demo\\netcore\\WebApplication4\\bin\\Debug\\netcoreapp3.1\\

            var modelPath = Option.ModelOutputPath + Delimiter + "Models";
            if (!Directory.Exists(modelPath))
            {
                Directory.CreateDirectory(modelPath);
            }

            var tableName = table.TableName;
            var className = table.TableName;

            var fullPath = modelPath + Delimiter + tableName + ".cs";
            if (File.Exists(fullPath) && !ifExistCovered)
                return;

            var pkTypeName = table.Columns.First(m => m.IsPrimaryKey).CSharpType;
            var sb = new StringBuilder();
            foreach (var column in table.Columns)
            {
                var tmp = GenerateEntityProperty(column);
                sb.AppendLine(tmp);
                sb.AppendLine();
            }
            var content = ReadTemplate("Model.txt");
            content = content.Replace("{ModelsNamespace}", Option.ModelsNamespace)
                .Replace("{Comment}", table.TableComment)
                .Replace("{TableName}", tableName)
                .Replace("{ModelName}", className)
                .Replace("{GeneratorTime}",Option.GeneratorTime)
                .Replace("{Author}",Option.Author)
                .Replace("{KeyTypeName}", pkTypeName)
                .Replace("{ModelProperties}", sb.ToString());
            WriteAndSave(fullPath, content);
        }

        private string GenerateEntityProperty(DbTableColumn column)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(column.Comment))
            {
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// " + column.Comment);
                sb.AppendLine("\t\t/// </summary>");
            }
            if (column.IsPrimaryKey)
            {
                sb.AppendLine("\t\t[Key]");
                sb.AppendLine($"\t\t[Column(\"{column.ColName}\")]");
                if (column.IsIdentity)
                {
                    sb.AppendLine("\t\t[DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }
                sb.AppendLine($"\t\tpublic {column.CSharpType} Id " + "{get;set;}");
            }
            else
            {

                sb.AppendLine($"\t\t[Column(\"{column.ColName}\")]");

                if (!column.IsNullable)
                {
                    sb.AppendLine("\t\t[Required]");
                }

                var colType = column.CSharpType;
                if (colType.ToLower() == "string" && column.ColumnLength.HasValue && column.ColumnLength.Value > 0)
                {
                    sb.AppendLine($"\t\t[MaxLength({column.ColumnLength.Value})]");
                }
                if (column.IsIdentity)
                {
                    sb.AppendLine("\t\t[DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }

                if (colType.ToLower() != "string" && colType.ToLower() != "byte[]" && colType.ToLower() != "object" &&
                    column.IsNullable)
                {
                    colType = colType + "?";
                }
                var colName = column.ColName;
                sb.AppendLine($"\t\tpublic {colType} {colName} " + "{get;set;}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public void WriteAndSave(string fileName, string content)
        {
            //实例化一个文件流--->与写入文件相关联
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                //实例化一个StreamWriter-->与fs相关联
                using (var sw = new StreamWriter(fs))
                {
                    //开始写入
                    sw.Write(content);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
            }
        }

    }
}
