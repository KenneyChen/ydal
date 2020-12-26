using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YDal.EntityFramework;

namespace NetCore.Dal.Interceptor
{
    public class SqlServerInterceptor : DbCommandInterceptor
    {
        #region Fields

        public bool IsEfSqlTracking;

        public SqlServerInterceptor(bool isEfSqlTracking)
        {
            IsEfSqlTracking = isEfSqlTracking;
        }


        /// <summary>
        /// 分析读取的表数目
        /// </summary>
        private static readonly Regex tableAliasRegex = new Regex(@"(?<tableAlias>\[dbo\].\[[^\]]+\] AS \[[^\]]+\])",
            RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 分析读取的字段数目 Regex
        /// </summary>
        private static readonly Regex columnsRegex =
            new Regex(
                @"SELECT\s+(?<top>TOP\s+\(\d+\)\s+)*(?<selectd>(?<columns>\[[^\]]+\].\[[^\]]+\]\sAS\s\[[^\]]+\]),*\s*)+FROM",
                RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Stopwatch _stopWatch = new Stopwatch();

        #endregion

        #region Unlock Table

        private bool _setUnlock = true;

        public bool SetUnlock
        {
            get
            {
                return _setUnlock;
            }
            set
            {
                _setUnlock = value;
            }
        }

        #endregion

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {

            //TransactionRepairInfo 事务补偿表，不应该用 nolock
            if (SetUnlock && (command.Transaction == null || command.Transaction.IsolationLevel < IsolationLevel.ReadCommitted) && (command.CommandText.IndexOf("[TransactionRepairInfo]", 0, System.StringComparison.InvariantCultureIgnoreCase) == -1))
            {
                //添加定位EF语句方法位置注释
                var info = GetStackTrackInfo();
                if (!string.IsNullOrEmpty(info))
                    command.CommandText = "/*" + info + "*/\n" + command.CommandText;
            }
            
     

            return base.ReaderExecuting(command, eventData, result);
        }

        //public override void ReaderExecuting(DbCommand command,
        //    DbCommandInterceptionContext<DbDataReader> interceptionContext)
        //{
        //    //TransactionRepairInfo 事务补偿表，不应该用 nolock
        //    if (SetUnlock && (command.Transaction == null || command.Transaction.IsolationLevel < IsolationLevel.ReadCommitted) && (command.CommandText.IndexOf("[TransactionRepairInfo]", 0, System.StringComparison.InvariantCultureIgnoreCase) == -1))
        //    {
        //        var dbContext = interceptionContext.DbContexts.FirstOrDefault() as DalDbContext;
        //        //if (dbContext != null && dbContext.AssignTableIndexs != null)
        //        //{
        //        //    command.CommandText = tableAliasRegex.Replace(command.CommandText, new MatchEvaluator((m) =>
        //        //    {
        //        //        //foreach (var index in dbContext.AssignTableIndexs)
        //        //        //{
        //        //        //    if (m.Value.IndexOf($"[dbo].[{index.Key}]") != -1)
        //        //        //    {
        //        //        //        return $"{m.Value} WITH(NOLOCK, INDEX={index.Value})";
        //        //        //    }
        //        //        //}

        //        //        return $"{m.Value} WITH(NOLOCK)";
        //        //    }));
        //        //    //这里无法区别Queryable是否有改变，所以不能确认要不要移除已使用的索引，暂时对于统计类SQL不移除指定索引
        //        //    //if (command.CommandText.IndexOf("COUNT(1) AS [A1]") == -1)
        //        //    //    dbContext.AssignTableIndexs = null;
        //        //}
        //        //else
        //        //{
        //            command.CommandText = tableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH(NOLOCK)");
        //        //}

        //        //添加定位EF语句方法位置注释
        //        var info = GetStackTrackInfo();
        //        if (!string.IsNullOrEmpty(info))
        //            command.CommandText = "/*" + info + "*/\n" + command.CommandText;
        //    }
        //    base.ReaderExecuting(command, interceptionContext);
        //    if (IsEfSqlTracking)
        //    {
        //        if (command.CommandText.ToLower().Contains("syssqltracking")) return;
        //        _stopWatch.Restart();
        //    }
        //}

        //public override void ReaderExecuted(DbCommand command,
        //    DbCommandInterceptionContext<DbDataReader> interceptionContext)
        //{
        //    if (IsEfSqlTracking)
        //    {
        //        _stopWatch.Stop();
        //        //SqlAnalysis(command.CommandText, _stopWatch.ElapsedMilliseconds);
        //    }
        //    base.ReaderExecuted(command, interceptionContext);
        //}

        #region SQL分析

        //private void SqlAnalysis(string sql, long executeMilliseconds)
        //{
        //    var item = new AnalysisItem();

        //    // 表数量
        //    var matches = tableAliasRegex.Matches(sql);
        //    item.Tables = matches.Count;

        //    // 列数量
        //    var match = columnsRegex.Match(sql);
        //    if (match.Success)
        //    {
        //        item.Columns = match.Groups["columns"].Captures.Count;
        //    }

        //    // SQL 语句长度
        //    item.SqlLength = sql.Length;

        //    // 调用堆栈
        //    //var trace = new StackTrace(true);
        //    //var builder = new StringBuilder();
        //    //foreach (var frame in trace.GetFrames())
        //    //{
        //    //    // 只读取自定义的类型信息
        //    //    var fileLineNumber = frame.GetFileLineNumber();
        //    //    if (fileLineNumber == 0)
        //    //    {
        //    //        continue;
        //    //    }
        //    //    builder.AppendFormat("{0} {1}: {2}", frame.GetFileName(), fileLineNumber, frame.GetMethod().Name);
        //    //    builder.AppendLine();
        //    //}
        //    var context = HttpContext.Current;
        //    if (context != null)
        //    {
        //        item.Url = context.Request.RawUrl;
        //    }
        //    else
        //    {
        //        item.Url = "";
        //    }
        //    //item.StackTrace = builder.ToString();

        //    // 执行时间
        //    item.ExecuteMilliseconds = executeMilliseconds;

        //    // SQL
        //    item.Sql = sql;

        //    Log4Logger.SqlTrackInfo(item);
        //}

        #endregion

        private string GetStackTrackInfo()
        {
            var trace = new StackTrace();
            return trace.ToString().Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(v => v.IndexOf("Kulv.YCF.Core") != -1);
        }
    }


    //public static class InterceptorConfiguration
    //{
    //    private static bool _configured;

    //    public static void Configure(bool isEfSqlTracking)
    //    {
    //        if (_configured) return;
    //        DbInterception.Add(new SqlServerInterceptor(isEfSqlTracking));
    //        _configured = true;
    //    }
    //}
}
