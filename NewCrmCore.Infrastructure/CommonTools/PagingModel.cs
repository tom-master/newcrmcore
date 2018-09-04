using System;
using System.Collections.Generic;
using System.Text;

namespace NewCrmCore.Infrastructure.CommonTools
{
    /// <summary>
    /// 分页模型
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class PageList<TModel>
    {
        public Int32 TotalCount { get; set; }

        public IList<TModel> Models { get; set; }
    }
}
