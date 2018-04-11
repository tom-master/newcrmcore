using System;
using System.Threading.Tasks;

namespace NewCrmCore.Domain.Services.Interface
{
	public interface ISkinContext
    {
        /// <summary>
        /// 修改皮肤
        /// </summary>
        Task ModifySkinAsync(Int32 accountId, String newSkin);
    }
}
