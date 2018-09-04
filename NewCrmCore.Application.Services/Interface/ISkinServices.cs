using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewCrmCore.Application.Services.Interface
{
    public interface ISkinServices
    {
        /// <summary>
        /// 获取全部的皮肤
        /// </summary>
        /// <param name="skinPath"></param>
        /// <returns></returns>
        Task<IDictionary<String, dynamic>> GetAllSkinAsync(String skinPath);

        /// <summary>
        /// 修改默认显示的皮肤
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="newSkin"></param>
        Task ModifySkinAsync(Int32 accountId, String newSkin);
    }
}
