using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewCRM.Domain.Services.Interface
{
    public interface ISkinContext
    {
        /// <summary>
        /// 修改皮肤
        /// </summary>
        Task ModifySkinAsync(Int32 accountId, String newSkin);
    }
}
