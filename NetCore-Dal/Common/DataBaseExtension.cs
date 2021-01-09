using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using YDal.Common;
using System.Linq;
using System.Collections;

namespace YDal.Common
{
    /// <summary>
    /// 扩展System.Data.Entity.Database
    /// </summary>
    public static class DataBaseExtension
    {
        public static int ExecuteSqlCommandWithNoLock(this DatabaseFacade database, string sql, params object[] parameters)
        {
            sql = "set transaction isolation level read uncommitted;" + sql;
            return database.ExecuteSqlCommand(sql, parameters);
        }

        public static IEnumerable<TElement> SqlQueryWithNoLock<TElement>(this DatabaseFacade database, string sql, params object[] parameters)
        {

            sql = "set transaction isolation level read uncommitted;" + sql;
            return database.ExecuteQuery<TElement>(sql, CommandType.Text, null, parameters);
        }

        public static DataSet SqlQueryWithNoLock(this DatabaseFacade database, string sql, params object[] parameters)
        {
            sql = "set transaction isolation level read uncommitted;" + sql;
            return database.ExecuteQuery(sql, CommandType.Text, parameters);

        }

        public static int SqlQuery(this DatabaseFacade db, string sql, CommandType commandType, params object[] parameters)
        {

            using (var conn = db.GetDbConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    conn.Open();
                    cmd.CommandText = sql;
                    cmd.CommandType = commandType;
                    return cmd.ExecuteNonQuery();
                }
            }
        }



        public static IEnumerable<TElement> SqlQueryProWithNoLock<TElement>(this DatabaseFacade database, string sql, params object[] parameters)
        {

            using (var conn = database.GetDbConnection())
            {
                conn.Open();
                //设置为未提交读隔离级别
                conn.BeginTransaction(IsolationLevel.ReadUncommitted).Commit();

                var ds = new DataSet();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }

                    using (var adapter = new SqlDataAdapter((SqlCommand)cmd))
                    {
                        adapter.Fill(ds);
                    }
                }

                if (ds.Tables.Count == 0)
                {
                    return new List<TElement>();
                }

                return ds.Tables[0].ToModels<TElement>(null);
            }


        }

        private static IEnumerable<T> ExecuteQuery<T>(this DatabaseFacade database, string command, CommandType type = CommandType.Text, Action<T> setter = null, IEnumerable<object> parameters = null)
        {
            var ds = ExecuteQuery(database, command, type, parameters);
            if (ds.Tables.Count == 0)
            {
                return new List<T>();
            }
            return ds.Tables[0].ToModels<T>(null);
        }

        public static Tuple<IList<T1>, IList<T2>> SqlQueryWithNoLock<T1, T2>(this DatabaseFacade database, string sql, IEnumerable<object> parameters = null)
            where T1 : class, new()
            where T2 : class, new()
        {
            sql = "set transaction isolation level read uncommitted;" + sql;
            var ds = ExecuteQuery(database, sql, CommandType.Text, parameters);
            IList<T1> t1 = ds.Tables[0].ToModels<T1>(null).ToList();
            IList<T2> t2 = ds.Tables[1].ToModels<T2>(null).ToList();
            return new Tuple<IList<T1>, IList<T2>>(t1, t2);
        }

        public static DataSet ExecuteQuery(this DatabaseFacade database, string command, CommandType type = CommandType.Text, IEnumerable<object> parameters = null)
        {

            using (var conn = database.GetDbConnection())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = command;
                    cmd.CommandType = type;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }

                    var ds = new DataSet();
                    using (var adapter = new SqlDataAdapter((SqlCommand)cmd))
                    {
                        adapter.Fill(ds);
                    }

                    return ds;
                }
            }
        }

        static T ToModel<T>(this DataRow row, Action<T, DataRow> setter = null)
        {
            Type type = typeof(T);
            T model = Activator.CreateInstance<T>();
            var properties = type.GetProperties();
            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo p = properties.FirstOrDefault(o => string.Equals(o.Name, column.ColumnName, StringComparison.InvariantCultureIgnoreCase));
                if (p != null)
                {
                    var value = row[column.ColumnName];
                    if (value == DBNull.Value)
                    {
                        value = null;
                        p.SetValue(model, null, null);
                    }
                    else
                    {
                        try
                        {
                            Type underlyingType = Nullable.GetUnderlyingType(p.PropertyType);
                            Type usedTargetType = underlyingType ?? p.PropertyType;
                            if (usedTargetType.IsEnum)
                            {
                                value = Enum.Parse(usedTargetType, value.ToString());
                            }
                            else if (usedTargetType == typeof(Guid))
                            {
                                value = Guid.Parse(value.ToString());
                            }
                            p.SetValue(model, value, null);
                        }
                        catch (Exception)
                        {
                            value = value.As(p.PropertyType);
                            p.SetValue(model, value, null);
                        }
                    }
                }
            }
            if (setter != null) setter(model, row);
            return model;
        }

        static IEnumerable<T> ToModels<T>(this DataTable datatable, Action<T, DataRow> setter = null)
        {
            foreach (DataRow row in datatable.Rows)
            {
                yield return row.ToModel(setter);
            }
        }


        #region connection对象
        public static int SqlQuery(this IDbConnection dbConnection, string sql, params object[] parameters)
        {
            return SqlQuery(dbConnection, sql,CommandType.Text, parameters);
        }

        public static int SqlQuery(this IDbConnection dbConnection, string sql, CommandType commandType, params object[] parameters)
        {

            using (var conn = dbConnection)
            {
                using (var cmd = conn.CreateCommand())
                {
                    //解决The SqlParameter is already contained by another SqlParameterCollection
                    //如果同时执行query+count语句会报错
                    var cloneParamters = parameters.Select(x =>((ICloneable) x).Clone()).ToArray();
                    foreach (var parameter in cloneParamters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    conn.Open();
                    cmd.CommandText = sql;
                    cmd.CommandType = commandType;
                    return cmd.ExecuteScalar().CastTo<int>();
                }
            }
        }

        /// <summary>
        /// 查询->对象返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this IDbConnection database, string commandText, CommandType type = CommandType.Text, IEnumerable<object> parameters = null)
        {
            return QueryDataSet(database, commandText, type, parameters).Tables[0].ToModels<T>();
        }

        /// <summary>
        /// 查询 datatable返回
        /// </summary>
        /// <param name="database"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable QueryTable(this IDbConnection database, string commandText, CommandType type = CommandType.Text, IEnumerable<object> parameters = null)
        {
            return QueryDataSet(database, commandText, type, parameters).Tables[0];
        }

        ///// <summary>
        ///// 查询 datatable返回
        ///// </summary>
        ///// <param name="database"></param>
        ///// <param name="commandText"></param>
        ///// <param name="type"></param>
        ///// <param name="parameters"></param>
        ///// <returns></returns>
        //public static IEnumerable<dynamic> QueryDynamic(this IDbConnection database, string commandText, CommandType type = CommandType.Text, IEnumerable<object> parameters = null)
        //{
        //    return QueryDataSet(database, commandText, type, parameters).Tables[0].AsDynamicEnumerable();
        //}

        /// <summary>
        /// 查询 dataset返回
        /// </summary>
        /// <param name="database"></param>
        /// <param name="commandText"></param>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet QueryDataSet(this IDbConnection database, string commandText, CommandType type = CommandType.Text, IEnumerable<object> parameters = null,int? page=null,int? pageSize=null)
        {
            using (var conn = database)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = commandText;
                    cmd.CommandType = type;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }

                    var ds = new DataSet();
                    using (var adapter = new SqlDataAdapter((SqlCommand)cmd))
                    {
                        adapter.Fill(ds);
                    }

                    return ds;
                }
            }
        }
        #endregion

        private static string BuildPageSql(this IDbConnection database,string sql) 
        {
         /*
          * select * from (
    　　　    　select *, ROW_NUMBER() OVER(Order by ArtistId ) AS RowId from ArtistModels
    　　    ) as b
          where RowId between 10 and 20
         */

            return "";
        }
    }

    public  static class DataTableExtension
    {
        //public static IEnumerable<dynamic> AsDynamicEnumerable(this DataTable table)
        //{
        //    // Validate argument here..
        //    return table.AsEnumerable().Select(row => new DynamicRow(row));
        //}

        //private sealed class DynamicRow : DynamicObject
        //{
        //    private readonly DataRow _row;

        //    internal DynamicRow(DataRow row) { _row = row; }

        //    // Interprets a member-access as an indexer-access on the 
        //    // contained DataRow.
        //    public override bool TryGetMember(GetMemberBinder binder, out object result)
        //    {
        //        var retVal = _row.Table.Columns.Contains(binder.Name);
        //        result = retVal ? _row[binder.Name] : null;
        //        return retVal;
        //    }
        //}
    }
}
