using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Dal.Models
{
    public class PagingInfo
    {
        public PagingInfo()
        {
            PageIndex = 1;
            NeedPage = true;
        }

        /// <summary>
        /// 是否需要分页，用于控制是否分页，默认为true
        /// </summary>
        public bool NeedPage { get; set; } = true;
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }
       
    }

}
