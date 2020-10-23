using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetCore.Dal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Kulv.YCF.Component
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

        public static void SqlQuery(this DatabaseFacade database, string sql, CommandType commandType, params object[] parameters)
        {

            using (var conn = database.GetDbConnection())
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
                    cmd.ExecuteNonQuery();
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

        static DataSet ExecuteQuery(this DatabaseFacade database, string command, CommandType type = CommandType.Text, IEnumerable<object> parameters = null)
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
    }
}
