using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;

namespace YDal.UnitOfWork.Impl
{
    /// <summary>
    ///     单元操作实现基类
    /// </summary>
    public abstract class BaseUnitOfWorkContext : IUnitOfWorkContext
    {
        /// <summary>
        /// 获取 当前使用的数据访问上下文对象
        /// </summary>
        protected abstract DbContext Context { get; }

        /// <summary>
        ///     获取 当前单元操作是否已被提交
        /// </summary>
        public bool IsCommitted { get; private set; }

        public DbContext DbContext { get { return Context; } }

        /// <summary>
        /// 提交当前单元操作的结果
        /// </summary>
        /// <param name="validateOnSaveEnabled">保存时是否自动验证跟踪实体</param>
        /// <returns></returns>
        public int Commit(bool validateOnSaveEnabled = true)
        {
            //validateOnSaveEnabled=true 插入才会返回赋值自增主键
            //validateOnSaveEnabled = false;
            if (IsCommitted)
            {
                return 0;
            }
            try
            {
                int result = Context.SaveChanges(validateOnSaveEnabled);

                IsCommitted = true;
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw;
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null && e.InnerException.InnerException is SqlException)
                {
                    var sqlEx = e.InnerException.InnerException as SqlException;
                    //string msg = DataHelper.GetSqlExceptionMessage(sqlEx.Number);
                    throw sqlEx; //PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + msg, sqlEx);
                }
                throw;
            }

        }

        /// <summary>
        ///  把当前单元操作标记成未提交状态,发生错误时，不能回滚事务，不要调用!
        /// </summary>
        public void Rollback()
        {
            IsCommitted = false;
        }

        public void Dispose()
        {
            //if (!IsCommitted)
            //{
            //    Commit();
            //}
            //if (Context != null)
            //    Context.Dispose();
        }

        /// <summary>
        /// EF SQL 语句返回 dataTable
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet SqlQueryForDataSet(CommandType commandType, string sql, SqlParameter[] parameters)
        {
            using (var conn = this.DbContext.Database.GetDbConnection())
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                var cmd = new SqlCommand { Connection = (SqlConnection)conn, CommandText = sql, CommandType = commandType, CommandTimeout = conn.ConnectionTimeout };
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }

                var adapter = new SqlDataAdapter(cmd);
                var ds = new DataSet();
                adapter.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }
    }
}
