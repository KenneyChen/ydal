using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace YDal.UnitOfWork
{
    /// <summary>
    ///     数据单元操作接口
    /// </summary>
    public interface IUnitOfWorkContext : IUnitOfWork, IDisposable
    {
        DataSet SqlQueryForDataSet(CommandType commandType, string sql, SqlParameter[] parameters);
    }
}
