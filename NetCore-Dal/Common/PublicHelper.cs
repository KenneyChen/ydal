using System;
using System.Collections.Generic;
using System.Text;

namespace YDal.Common
{
    /// <summary>
    ///     公共辅助操作类
    /// </summary>
    public static class PublicHelper
    {
        #region 公共方法

        /// <summary>
        ///     检验参数合法性，数值类型不能小于0，引用类型不能为null，否则抛出相应异常
        /// </summary>
        /// <param name="arg"> 待检参数 </param>
        /// <param name="argName"> 待检参数名称 </param>
        /// <param name="canZero"> 数值类型是否可以等于0 </param>
        /// <exception cref="ComponentException" />
        public static void CheckArgument(object arg, string argName, bool canZero = false)
        {
            if (arg == null)
            {
                var e = new ArgumentNullException(argName);
                throw new Exception(string.Format("参数 {0} 为空引发异常。", argName), e);
            }
            Type type = arg.GetType();
            if (type.IsValueType && type.IsNumeric())
            {
                bool flag = !canZero ? arg.CastTo(0.0) <= 0.0 : arg.CastTo(0.0) < 0.0;
                if (flag)
                {
                    var e = new ArgumentOutOfRangeException(argName);
                    throw new Exception(string.Format("参数 {0} 不在有效范围内引发异常。具体信息请查看系统日志。", argName), e);
                }
            }
            if (type == typeof(Guid) && (Guid)arg == Guid.Empty)
            {
                var e = new ArgumentNullException(argName);
                throw new Exception(string.Format("参数{0}为空Guid引发异常。", argName), e);
            }
        }

        

        #endregion
    }
}
