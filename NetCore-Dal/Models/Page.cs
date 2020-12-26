using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Dal.Models
{
    public class Page<T>
    {
        /// <summary>
        /// 当前总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public List<T> Records { get; set; }    
    }

    public class PageObject<T>
    {
        /// <summary>
        /// 当前总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public Object Records { get; set; }
    }
}
