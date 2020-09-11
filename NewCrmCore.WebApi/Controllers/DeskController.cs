using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Infrastructure;
using NewCrmCore.WebApi.ApiHelper;
using NewLibCore.Validate;
using NewLibCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;

namespace NewCrmCore.WebApi.Controllers
{
    public class DeskController : NewCrmController
    {
        private readonly IDeskServices _deskServices;

        private readonly IAppServices _appServices;

        private readonly IUserServices _userServices;

        private readonly IWallpaperServices _wallpaperServices;

        public DeskController(IWallpaperServices wallpaperServices,
        IDeskServices deskServices,
        IAppServices appServices,
        IUserServices userServices)
        {
            _wallpaperServices = wallpaperServices;
            _deskServices = deskServices;
            _appServices = appServices;
            _userServices = userServices;
        }

        #region 页面

        /// <summary>
        /// 桌面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InitDeskInfoAsync()
        {
            var response = new ResponseModel<dynamic>();
            var userContext = await GetUserContextAsync();
            if (userContext != null)
            {
                var config = await _userServices.GetConfigAsync(userContext.Id);
                if (config == null)
                {
                    response.IsSuccess = true;
                    response.Message = "获取桌面配置信息失败";
                }
                else
                {
                    response.Model = new { userContext, config };
                    response.IsSuccess = true;
                    response.Message = "初始化桌面信息成功";
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "会话过期,请重新登陆";
            }
            return Json(response);
        }

        /// <summary>
        /// 配置用户应用
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfigMemberAsync(Int32 memberId)
        {
            #region 参数验证
            Parameter.IfNullOrZero(memberId);
            #endregion
            var userContext = await GetUserContextAsync();
            var member = await _deskServices.GetMemberAsync(userContext.Id, memberId);
            var uniqueToken = CreateUniqueTokenAsync(userContext.Id);

            return Json(new ResponseModel<dynamic>
            {
                Model = new { member, uniqueToken },
                IsSuccess = true,
                Message = "配置用户应用初始化成功"
            });
        }

        /// <summary>
        /// 系统壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SystemWallPaperAsync()
        {
            var userContext = await GetUserContextAsync();
            var config = await _userServices.GetConfigAsync(userContext.Id);
            var wallpapers = await _wallpaperServices.GetWallpapersAsync();
            return Json(new ResponseModel<dynamic>
            {
                Model = new { config, wallpapers },
                IsSuccess = true,
                Message = "获取系统壁纸成功"
            });
        }

        /// <summary>
        /// 自定义壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CustomizeWallpaperAsync()
        {
            var userContext = await GetUserContextAsync();
            var config = await _userServices.GetConfigAsync(userContext.Id);
            return Json(new ResponseModel<dynamic>
            {
                Model = new { config },
                IsSuccess = true,
                Message = "自定义壁纸初始化成功"
            });
        }

        /// <summary>
        /// 桌面设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfigDeskAsync()
        {
            var userContext = await GetUserContextAsync();
            var config = await _userServices.GetConfigAsync(userContext.Id);
            return Json(new ResponseModel<dynamic>
            {
                Model = new { config },
                IsSuccess = true,
                Message = "桌面设置初始化成功"
            });
        }


        #endregion

        #region 登陆

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="loginParameter"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> LoginAsync(UserLogin loginParameter)
        {
            #region 参数验证
            Parameter.IfNullOrZero(loginParameter);
            #endregion
            var response = new ResponseModel<UserDto>();

            var user = await _userServices.LoginAsync(loginParameter.UserName, loginParameter.Password, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (user != null)
            {
                response.Message = "登陆成功";
                response.IsSuccess = true;

                var cookieTimeout = (loginParameter.Remember) ? DateTime.Now.AddDays(7) : DateTime.Now.AddHours(1);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim(JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(cookieTimeout).ToUnixTimeSeconds()}"),
                    new Claim("UserName",user.Name),
                    new Claim("UserId",user.Id.ToString()),
                    new Claim("IsAdmin",user.IsAdmin.ToString()),
                    new Claim("UserFace",user.UserFace)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Appsetting.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: Appsetting.Domain,
                    audience: Appsetting.Domain,
                    claims: claims,
                    expires: cookieTimeout,
                    signingCredentials: creds);
                response.Token = new JwtSecurityTokenHandler().WriteToken(token);
                response.Model = user;
            }
            else
            {
                response.Message = "登陆失败";
                response.IsSuccess = false;
            }
            return Json(response);
        }

        #endregion

        #region 设置壁纸

        /// <summary>
        /// 设置壁纸
        /// </summary>
        /// <param name="wallpaperId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyWallpaperAsync(Int32 wallpaperId)
        {
            #region 参数验证
            Parameter.IfNullOrZero(wallpaperId);
            #endregion
            var userContext = await GetUserContextAsync();
            await _wallpaperServices.ModifyWallpaperAsync(userContext.Id, wallpaperId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "设置壁纸成功"
            });
        }

        #endregion

        #region 删除壁纸

        /// <summary>
        /// 删除壁纸
        /// </summary>
        /// <param name="wallPaperId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveWallpaperAsync(Int32 wallPaperId)
        {
            #region 参数验证
            Parameter.IfNullOrZero(wallPaperId);
            #endregion

            var userContext = await GetUserContextAsync();
            await _wallpaperServices.RemoveWallpaperAsync(userContext.Id, wallPaperId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "删除壁纸成功"
            });
        }

        #endregion

        #region 上传壁纸

        /// <summary>
        /// 上传壁纸
        /// </summary>
        /// <param name="wallpaper"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadWallPaperAsync(WallpaperDto wallpaper)
        {
            #region 参数验证
            Parameter.IfNullOrZero(wallpaper);
            #endregion
            var userContext = await GetUserContextAsync();
            var (wapperId, url) = await _wallpaperServices.AddWallpaperAsync(new WallpaperDto
            {
                Title = wallpaper.Title.Substring(0, 9),
                Width = wallpaper.Width,
                Height = wallpaper.Height,
                Url = wallpaper.Url,
                Source = wallpaper.Source,
                UserId = userContext.Id,
                Md5 = wallpaper.Md5,
                ShortUrl = ""
            });

            return Json(new ResponseModel<dynamic>
            {
                Message = "壁纸上传成功",
                IsSuccess = true,
                Model = new
                {
                    Id = wapperId,
                    Url = Appsetting.FileUrl + url
                }
            });
        }

        #endregion

        #region 网络壁纸

        /// <summary>
        /// 网络壁纸
        /// </summary>
        /// <param name="webUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> WebWallpaperAsync(String webUrl)
        {
            #region 参数验证
            Parameter.IfNullOrZero(webUrl);
            #endregion
            var userContext = await GetUserContextAsync();
            var result = _wallpaperServices.AddWebWallpaperAsync(userContext.Id, webUrl);
            return Json(new ResponseModel<(Int32 wapperId, String url)>
            {
                IsSuccess = true,
                Message = "网络壁纸保存成功",
                Model = await result
            });
        }

        #endregion

        #region 更换皮肤

        /// <summary>
        /// 更换皮肤
        /// </summary>
        /// <param name="skin"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifySkinAsync(String skin)
        {
            #region 参数验证
            Parameter.IfNullOrZero(skin);
            #endregion
            var userContext = await GetUserContextAsync();
            await _deskServices.ModifySkinAsync(userContext.Id, skin);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更换皮肤成功"
            });
        }

        #endregion

        #region 更新图标

        /// <summary>
        /// 更新图标
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyIconAsync(ModifyIconForMember model)
        {
            #region 参数验证
            Parameter.IfNullOrZero(model.MemberId);
            Parameter.IfNullOrZero(model.NewIcon);
            #endregion
            var userContext = await GetUserContextAsync();
            await _deskServices.ModifyMemberIconAsync(userContext.Id, model.MemberId, model.NewIcon);
            return Json(new ResponseModel<String>
            {
                IsSuccess = true,
                Message = "更新图标成功",
                Model = $"{Appsetting.FileUrl}{model.NewIcon}"
            });
        }

        #endregion

        #region 解锁屏幕

        /// <summary>
        /// 解锁屏幕
        /// </summary>
        /// <param name="unlockPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnlockScreenAsync(String unlockPassword)
        {
            #region 参数验证
            Parameter.IfNullOrZero(unlockPassword);
            #endregion

            var userContext = await GetUserContextAsync();
            var response = new ResponseSimple();
            var result = await _userServices.UnlockScreenAsync(userContext.Id, unlockPassword);
            if (result)
            {
                response.Message = "屏幕解锁成功";
                response.IsSuccess = true;
            }

            return Json(response);
        }

        #endregion

        #region 账户登出

        /// <summary>
        /// 账户登出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            var userContext = await GetUserContextAsync();
            await _userServices.LogoutAsync(userContext.Id);
            InternalLogout();
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "注销登陆成功"
            });
        }

        #endregion

        #region 获取皮肤

        /// <summary>
        /// 获取皮肤
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSkinAsync()
        {
            var userContext = await GetUserContextAsync();
            var skinName = (await _userServices.GetConfigAsync(userContext.Id)).Skin;
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Model = skinName,
                Message = "初始化皮肤成功"
            });
        }

        #endregion

        #region 获取壁纸

        /// <summary>
        /// 获取壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetWallpaperAsync()
        {
            var userContext = await GetUserContextAsync();
            var result = await _userServices.GetConfigAsync(userContext.Id);
            if (result.WallpaperSource == WallpaperSource.Upload)
            {
                result.WallpaperUrl = Appsetting.FileUrl + result.WallpaperUrl;
            }

            if (result.IsBing)
            {
                result.WallpaperSource = WallpaperSource.Bing;
                result.WallpaperUrl = await BingHelper.GetEverydayBackgroundImageAsync();
            }

            return Json(new ResponseModel<dynamic>
            {
                IsSuccess = true,
                Message = "初始化壁纸成功",
                Model = result
            });
        }

        #endregion

        #region 创建窗口

        /// <summary>
        /// 创建窗口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CreateWindowAsync(Int32 id, String type)
        {

            #region 参数验证
            Parameter.IfNullOrZero(id);
            Parameter.IfNullOrZero(type);
            #endregion

            var userContext = await GetUserContextAsync();
            var internalMemberResult = type == "folder" ? await _deskServices.GetMemberAsync(userContext.Id, id, true) : await _deskServices.GetMemberAsync(userContext.Id, id);

            var response = new ResponseModel<dynamic>
            {
                IsSuccess = true,
                Message = "创建一个窗口成功",
                Model = new CreateWindowDto
                {
                    type = internalMemberResult.MemberType.ToString(),
                    memberId = internalMemberResult.Id,
                    appId = internalMemberResult.AppId,
                    name = internalMemberResult.Name,
                    icon = internalMemberResult.IsIconByUpload ? $@"{Appsetting.FileUrl}{ internalMemberResult.IconUrl}" : internalMemberResult.IconUrl,
                    width = internalMemberResult.Width,
                    height = internalMemberResult.Height,
                    isOnDock = internalMemberResult.IsOnDock,
                    isOpenMax = internalMemberResult.IsOpenMax,
                    isSetbar = internalMemberResult.IsSetbar,
                    url = internalMemberResult.AppUrl,
                    isResize = internalMemberResult.IsResize,
                    starCount = internalMemberResult.StarCount
                }
            };

            return Json(response);
        }

        #endregion

        #region 新建文件夹

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateFolderAsync(CreateFolder model)
        {
            #region 参数验证
            Parameter.IfNullOrZero(model.FolderName);
            Parameter.IfNullOrZero(model.FolderImg);
            Parameter.IfNullOrZero(model.DeskId);
            #endregion

            var userContext = await GetUserContextAsync();
            await _deskServices.CreateNewFolderAsync(model.FolderName, model.FolderImg, model.DeskId, userContext.Id);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "新建文件夹成功"
            });
        }

        #endregion

        #region 卸载桌面应用

        /// <summary>
        /// 卸载桌面应用
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UninstallAsync(Int32 memberId)
        {
            #region 参数验证 
            Parameter.IfNullOrZero(memberId);
            #endregion

            var userContext = await GetUserContextAsync();
            await _deskServices.UninstallMemberAsync(userContext.Id, memberId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "卸载成功"
            });
        }

        #endregion

        #region 检查桌面应用名称

        /// <summary>
        /// 检查桌面应用名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckNameAsync(String param)
        {
            #region 参数验证
            Parameter.IfNullOrZero(param);
            #endregion

            var result = await _deskServices.CheckMemberNameAsync(param);
            var response = new ResponseSimple();
            if (!result)
            {
                response.Model = "y";
                response.IsSuccess = true;
                response.Message = "";
            }
            else
            {
                response.Model = "n";
                response.IsSuccess = false;
                response.Message = "应用名称已存在";
            }
            return Json(response);
        }

        #endregion

        #region 桌面应用移动

        /// <summary>
        /// 桌面应用移动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MemberMoveAsync(MemberMove model)
        {
            #region 参数验证
            Parameter.IfNullOrZero(model.MoveType);
            Parameter.IfNullOrZero(model.MemberId);
            #endregion
            var userContext = await GetUserContextAsync();
            switch (model.MoveType.ToLower())
            {
                case "desk-dock": //桌面应用从桌面移动到码头
                    await _deskServices.MemberInDockAsync(userContext.Id, model.MemberId);
                    break;
                case "dock-desk": //桌面应用从码头移动到桌面
                    await _deskServices.MemberOutDockAsync(userContext.Id, model.MemberId, model.To);
                    break;
                case "dock-folder": //桌面应用从码头移动到桌面文件夹中
                    await _deskServices.DockToFolderAsync(userContext.Id, model.MemberId, model.To);
                    break;
                case "folder-dock": //桌面应用从文件夹移动到码头
                    await _deskServices.FolderToDockAsync(userContext.Id, model.MemberId);
                    break;
                case "desk-folder": //桌面应用从桌面移动到文件夹
                    await _deskServices.DeskToFolderAsync(userContext.Id, model.MemberId, model.To);
                    break;
                case "folder-desk": //桌面应用从文件夹移动到桌面
                    await _deskServices.FolderToDeskAsync(userContext.Id, model.MemberId, model.To);
                    break;
                case "folder-folder": //桌面应用从文件夹移动到另一个文件夹中
                    await _deskServices.FolderToOtherFolderAsync(userContext.Id, model.MemberId, model.To);
                    break;
                case "desk-desk": //桌面移动到另一个桌面
                    await _deskServices.DeskToOtherDeskAsync(userContext.Id, model.MemberId, model.To);
                    break;
                case "dock-otherdesk"://应用码头移动到另一个桌面
                    await _deskServices.DockToOtherDeskAsync(userContext.Id, model.MemberId, model.To);
                    break;
            }

            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "移动成功"
            });
        }

        #endregion

        #region 修改桌面应用信息

        /// <summary>
        /// 修改桌面应用信息
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyMemberInfoAsync(IFormCollection forms)
        {
            #region 参数验证
            Parameter.IfNullOrZero(forms);
            #endregion
            var userContext = await GetUserContextAsync();
            var memberDto = new MemberDto
            {
                Id = Int32.Parse(forms["id"]),
                IconUrl = forms["val_icon"],
                Name = forms["val_name"],
                Width = Int32.Parse(forms["val_width"]),
                Height = Int32.Parse(forms["val_height"]),
                IsResize = Int32.Parse(forms["val_isresize"]) == 1,
                IsOpenMax = Int32.Parse(forms["val_isopenmax"]) == 1,
                IsFlash = Int32.Parse(forms["val_isflash"]) == 1,
                MemberType = EnumExtensions.ToEnum<MemberType>(forms["membertype"]),
                IsIconByUpload = Int32.Parse(forms["isIconByUpload"]) == 1
            };

            await _deskServices.ModifyMemberInfoAsync(userContext.Id, memberDto);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "修改桌面应用信息成功"
            });
        }

        #endregion

        #region 载入上传壁纸

        /// <summary>
        /// 载入上传壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUploadWallPapersAsync()
        {
            var userContext = await GetUserContextAsync();
            var result = await _wallpaperServices.GetUploadWallpaperAsync(userContext.Id);
            return Json(new ResponseModel<IList<WallpaperDto>>
            {
                IsSuccess = true,
                Message = "载入之前上传的壁纸成功",
                Model = result
            });
        }

        #endregion

        #region 获取全部皮肤

        /// <summary>
        /// 获取全部皮肤
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSkinsAsync()
        {
            var userContext = await GetUserContextAsync();
            var result = await _deskServices.GetAllSkinAsync(Appsetting.Skin);
            return Json(new ResponseModel<dynamic>
            {
                IsSuccess = true,
                Message = "获取皮肤列表成功",
                Model = new { result, currentSkin = (await _userServices.GetConfigAsync(userContext.Id)).Skin }
            });
        }

        #endregion

        #region 更改码头位置

        /// <summary>
        /// 更改码头位置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyDockPositionAsync(ModifyDockPosition model)
        {
            #region 参数验证
            Parameter.IfNullOrZero(model.Pos);
            Parameter.IfNullOrZero(model.DeskNum);
            #endregion

            var userContext = await GetUserContextAsync();
            await _deskServices.ModifyDockPositionAsync(userContext.Id, model.DeskNum, model.Pos);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更改码头的位置成功"
            });
        }

        #endregion

        #region 修改壁纸来源

        /// <summary>
        /// 修改壁纸来源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyWallpaperSourceAsync(String source)
        {
            #region 参数验证
            Parameter.IfNullOrZero(source);
            #endregion

            var userContext = await GetUserContextAsync();
            await _deskServices.ModifyWallpaperSourceAsync(source, userContext.Id);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更改壁纸来源成功"
            });
        }

        #endregion

        #region 更改图标大小

        /// <summary>
        /// 更改图标大小
        /// </summary>
        /// <param name="appSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifySizeAsync(Int32 appSize)
        {
            #region 参数验证
            Parameter.IfNullOrZero(appSize);
            #endregion

            var userContext = await GetUserContextAsync();
            await _appServices.ModifyAppIconSizeAsync(userContext.Id, appSize);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更改图标大小成功"
            });
        }

        #endregion

        #region 获取账户头像

        /// <summary>
        /// 获取账户头像
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserFaceAsync()
        {
            var userContext = await GetUserContextAsync();
            var result = (await _userServices.GetConfigAsync(userContext.Id)).UserFace;
            return Json(new ResponseModel<String>
            {
                IsSuccess = true,
                Message = "获取用户头像成功",
                Model = $"{Appsetting.FileUrl}{result}"
            });
        }

        #endregion

        #region 修改文件夹信息

        /// <summary>
        /// 修改文件夹信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyFolderInfoAsync(ModifyFolderInfo model)
        {
            #region 参数验证
            Parameter.IfNullOrZero(model.Name);
            Parameter.IfNullOrZero(model.Icon);
            Parameter.IfNullOrZero(model.MemberId);
            #endregion

            var userContext = await GetUserContextAsync();
            await _deskServices.ModifyFolderInfoAsync(userContext.Id, model.Name, model.Icon, model.MemberId);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "修改成功"
            });
        }

        #endregion

        #region 初始化应用码头

        /// <summary>
        /// 初始化应用码头
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDockPosAsync()
        {
            var userContext = await GetUserContextAsync();
            var result = await _userServices.GetConfigAsync(userContext.Id);
            return Json(new ResponseModel<ConfigDto>
            {
                IsSuccess = true,
                Message = "初始化应用码头成功",
                Model = result
            });
        }

        #endregion

        #region 更换默认显示桌面

        /// <summary>
        /// 更换默认显示桌面
        /// </summary>
        /// <param name="deskNum"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyDefaultDeskAsync(Int32 deskNum)
        {
            #region 参数验证
            Parameter.IfNullOrZero(deskNum);
            #endregion

            var userContext = await GetUserContextAsync();
            await _deskServices.ModifyDefaultDeskNumberAsync(userContext.Id, deskNum);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更换默认桌面成功"
            });
        }

        #endregion

        #region 更换图标排列方向

        /// <summary>
        /// 更换图标排列方向
        /// </summary>
        /// <param name="appXy"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyXyAsync(String appXy)
        {
            #region 参数验证
            Parameter.IfNullOrZero(appXy);
            #endregion

            var userContext = await GetUserContextAsync();
            await _appServices.ModifyAppDirectionAsync(userContext.Id, appXy);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更换图标排列方向成功",
            });
        }

        #endregion

        #region 设置壁纸显示模式

        /// <summary>
        /// 设置壁纸显示模式
        /// </summary>
        /// <param name="wallPaperShowType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyDisplayModelAsync(String wallPaperShowType)
        {
            #region 参数验证
            Parameter.IfNullOrZero(wallPaperShowType);
            #endregion

            var userContext = await GetUserContextAsync();
            await _wallpaperServices.ModifyWallpaperModeAsync(userContext.Id, wallPaperShowType);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "壁纸显示模式设置成功"
            });
        }

        #endregion

        #region 获取账户安装的应用

        /// <summary>
        /// 获取账户安装的应用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserDeskMembersAsync()
        {
            var userContext = await GetUserContextAsync();
            var result = await _deskServices.GetDeskMembersAsync(userContext.Id);
            return Json(new ResponseModel<IDictionary<String, IList<dynamic>>>
            {
                IsSuccess = true,
                Message = "获取我的应用成功",
                Model = result
            });
        }

        #endregion

        #region 更改图标的水平间距

        /// <summary>
        /// 更改图标的水平间距
        /// </summary>
        /// <param name="appHorizontal"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyHorizontalSpaceAsync(Int32 appHorizontal)
        {
            #region 参数验证
            Parameter.IfNullOrZero(appHorizontal);
            #endregion

            var userContext = await GetUserContextAsync();
            await _appServices.ModifyAppHorizontalSpacingAsync(userContext.Id, appHorizontal);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更改图标水平间距成功"
            });
        }

        #endregion

        #region 更改应用图标垂直间距

        /// <summary>
        /// 更改应用图标垂直间距
        /// </summary>
        /// <param name="appVertical"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyVerticalSpaceAsync(Int32 appVertical)
        {
            #region 参数验证
            Parameter.IfNullOrZero(appVertical);
            #endregion

            var userContext = await GetUserContextAsync();
            await _appServices.ModifyAppVerticalSpacingAsync(userContext.Id, appVertical);
            return Json(new ResponseSimple
            {
                IsSuccess = true,
                Message = "更改图标垂直间距成功"
            });
        }

        #endregion

        /// <summary>
        /// 检查未读通知数量
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckUnreadNotifyCountAsync(Int32 pageIndex, Int32 pageSize)
        {
            #region 参数验证
            Parameter.IfNullOrZero(pageIndex);
            Parameter.IfNullOrZero(pageSize);
            #endregion

            var userContext = await GetUserContextAsync();
            var result = await _deskServices.CheckUnreadNotifyCount(userContext.Id, pageIndex, pageSize);
            return Json(new ResponseModel<PageList<NotifyDto>>
            {
                IsSuccess = true,
                Model = result,
                Message = "消息列表获取成功"
            });
        }

        /// <summary>
        /// 读取通知
        /// </summary>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadNotifyAsync(String notifyId)
        {
            #region 参数验证
            Parameter.IfNullOrZero(notifyId, true);
            #endregion

            var response = new ResponseSimple();

            if (String.IsNullOrEmpty(notifyId))
            {
                response.IsSuccess = false;
                response.Message = "消息读取失败";
                return Json(response);
            }

            await _deskServices.ReadNotify(notifyId.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList());
            response.IsSuccess = true;
            response.Message = "消息读取成功";

            return Json(response);
        }
    }
}