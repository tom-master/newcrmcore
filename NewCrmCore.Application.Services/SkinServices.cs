using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
    public class SkinServices : ISkinServices
    {
        private readonly ISkinContext _skinContext;

        public SkinServices(ISkinContext skinContext)
        {
            _skinContext = skinContext;
        }

        public async Task<IDictionary<String, dynamic>> GetAllSkinAsync(String skinPath)
        {
            Parameter.Validate(skinPath);

            return await Task.Run(() =>
            {
                IDictionary<String, dynamic> dataDictionary = new Dictionary<String, dynamic>();
                Directory.GetFiles(skinPath, "*.css").ToList().ForEach(path =>
                {
                    var fileName = Path.GetFileName(path);
                    fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                    var cssPath = path.Substring(path.LastIndexOf("images") - 1).Replace(@"\", "/");
                    dataDictionary.Add(fileName, new
                    {
                        cssPath,
                        imgPath = $@"{cssPath.Substring(0, cssPath.LastIndexOf("."))}/preview.png"
                    });
                });
                return dataDictionary;
            });
        }

        public async Task ModifySkinAsync(Int32 userId, String newSkin)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newSkin);
            await _skinContext.ModifySkinAsync(userId, newSkin);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }
    }
}
