using System;
using System.Threading.Tasks;

namespace NewCrmCore.Domain.Services.Interface
{
    public interface ISkinContext
    {
        /// <summary>
        /// 修改皮肤
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="newSkin"></param>
        /// <returns></returns>
        Task ModifySkinAsync(Int32 accountId, String newSkin);
    }
}
