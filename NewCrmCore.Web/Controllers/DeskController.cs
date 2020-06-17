using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore.Validate;
using NewLibCore;
using Newtonsoft.Json;

namespace NewCrmCore.Web.Controllers
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
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> InitDeskInfoAsync()
        {
            var response = new ResponseModel<dynamic>();
            if (UserInfo != null)
            {
                var config = await _userServices.GetConfigAsync(UserInfo.Id);
                if (config == null)
                {
                    response.IsSuccess = true;
                    response.Message = "获取桌面配置信息失败";
                    return Json(response);
                }
                response.Model = new { UserInfo, config };
                response.IsSuccess = true;
                response.Message = "初始化桌面信息成功";
                return Json(response);
            }

            return RedirectToAction("login", "desk");
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
            Parameter.Validate(memberId);
            #endregion

            var member = await _deskServices.GetMemberAsync(UserInfo.Id, memberId);
            var uniqueToken = CreateUniqueTokenAsync(UserInfo.Id);

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
            var config = await _userServices.GetConfigAsync(UserInfo.Id);
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
            var config = await _userServices.GetConfigAsync(UserInfo.Id);
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
            var config = await _userServices.GetConfigAsync(UserInfo.Id);
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
        public async Task<IActionResult> LandingAsync(UserLogin loginParameter)
        {
            #region 参数验证
            Parameter.Validate(loginParameter);
            #endregion
            var response = new ResponseModel<UserDto>();

            var user = await _userServices.LoginAsync(loginParameter.UserName, loginParameter.Password, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (user != null)
            {
                response.Message = "登陆成功";
                response.IsSuccess = true;

                var cookieTimeout = (loginParameter.Remember) ? DateTime.Now.AddDays(7) : DateTime.Now.AddMinutes(60);
                // var claims = new[]
                // {
                //     new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                //     new Claim(JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(cookieTimeout).ToUnixTimeSeconds()}"),
                //     new Claim("User",JsonConvert.SerializeObject(user))
                // };

                // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Appsetting.SecurityKey));
                // var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                // var token = new JwtSecurityToken(
                //     issuer: Appsetting.Domain,
                //     audience: Appsetting.Domain,
                //     claims: claims,
                //     expires: cookieTimeout,
                //     signingCredentials: creds);
                // response.Token = new JwtSecurityTokenHandler().WriteToken(token);
                Response.Cookies.Append("User", JsonConvert.SerializeObject(new { Id = user.Id, UserFace = user.UserFace, Name = user.Name }), new CookieOptions { Expires = cookieTimeout });
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
            Parameter.Validate(wallpaperId);
            #endregion

            await _wallpaperServices.ModifyWallpaperAsync(UserInfo.Id, wallpaperId);
            return Json(new ResponseModel
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
            Parameter.Validate(wallPaperId);
            #endregion

            await _wallpaperServices.RemoveWallpaperAsync(UserInfo.Id, wallPaperId);
            return Json(new ResponseModel
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
            Parameter.Validate(wallpaper);
            #endregion

            var wallpaperResult = await _wallpaperServices.AddWallpaperAsync(new WallpaperDto
            {
                Title = wallpaper.Title.Substring(0, 9),
                Width = wallpaper.Width,
                Height = wallpaper.Height,
                Url = wallpaper.Url,
                Source = wallpaper.Source,
                UserId = UserInfo.Id,
                Md5 = wallpaper.Md5,
                ShortUrl = ""
            });

            return Json(new ResponseModel<dynamic>
            {
                Message = "壁纸上传成功",
                IsSuccess = true,
                Model = new { Id = wallpaperResult.Item1, Url = Appsetting.FileUrl + wallpaperResult.Item2 }
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
            Parameter.Validate(webUrl);
            #endregion

            var result = _wallpaperServices.AddWebWallpaperAsync(UserInfo.Id, webUrl);
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
            Parameter.Validate(skin);
            #endregion

            await _deskServices.ModifySkinAsync(UserInfo.Id, skin);
            return Json(new ResponseModel
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
            Parameter.Validate(model.MemberId);
            Parameter.Validate(model.NewIcon);
            #endregion

            await _deskServices.ModifyMemberIconAsync(UserInfo.Id, model.MemberId, model.NewIcon);
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
            Parameter.Validate(unlockPassword);
            #endregion

            var response = new ResponseModel();
            var result = await _userServices.UnlockScreenAsync(UserInfo.Id, unlockPassword);
            if (result)
            {
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
            await _userServices.LogoutAsync(UserInfo.Id);
            InternalLogout();
            return Json(new ResponseModel
            {
                IsSuccess = true
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
            var skinName = (await _userServices.GetConfigAsync(UserInfo.Id)).Skin;
            return Json(new ResponseModel<String>
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
            var result = await _userServices.GetConfigAsync(UserInfo.Id);

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
                Model = new { result.WallpaperUrl, result.WallpaperSource, result.WallpaperHeigth, result.WallpaperMode, result.WallpaperWidth }
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
            Parameter.Validate(id);
            Parameter.Validate(type);
            #endregion

            var internalMemberResult = type == "folder" ? await _deskServices.GetMemberAsync(UserInfo.Id, id, true) : await _deskServices.GetMemberAsync(UserInfo.Id, id);

            var response = new ResponseModel<dynamic>
            {
                IsSuccess = true,
                Message = "创建一个窗口成功",
                Model = new
                {
                    type = internalMemberResult.MemberType,
                    memberId = internalMemberResult.Id,
                    appId = internalMemberResult.AppId,
                    name = internalMemberResult.Name,
                    icon = internalMemberResult.IsIconByUpload ? Appsetting.FileUrl + internalMemberResult.IconUrl : internalMemberResult.IconUrl,
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
            Parameter.Validate(model.FolderName);
            Parameter.Validate(model.FolderImg);
            Parameter.Validate(model.DeskId);
            #endregion

            await _deskServices.CreateNewFolderAsync(model.FolderName, model.FolderImg, model.DeskId, UserInfo.Id);
            return Json(new ResponseModel
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
            Parameter.Validate(memberId);
            #endregion

            await _deskServices.UninstallMemberAsync(UserInfo.Id, memberId);
            return Json(new ResponseModel
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
            Parameter.Validate(param);
            #endregion

            var result = await _deskServices.CheckMemberNameAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "应用名称已存在" });
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
            Parameter.Validate(model.MoveType);
            Parameter.Validate(model.MemberId);
            #endregion

            switch (model.MoveType.ToLower())
            {
                case "desk-dock": //桌面应用从桌面移动到码头
                    await _deskServices.MemberInDockAsync(UserInfo.Id, model.MemberId);
                    break;
                case "dock-desk": //桌面应用从码头移动到桌面
                    await _deskServices.MemberOutDockAsync(UserInfo.Id, model.MemberId, model.To);
                    break;
                case "dock-folder": //桌面应用从码头移动到桌面文件夹中
                    await _deskServices.DockToFolderAsync(UserInfo.Id, model.MemberId, model.To);
                    break;
                case "folder-dock": //桌面应用从文件夹移动到码头
                    await _deskServices.FolderToDockAsync(UserInfo.Id, model.MemberId);
                    break;
                case "desk-folder": //桌面应用从桌面移动到文件夹
                    await _deskServices.DeskToFolderAsync(UserInfo.Id, model.MemberId, model.To);
                    break;
                case "folder-desk": //桌面应用从文件夹移动到桌面
                    await _deskServices.FolderToDeskAsync(UserInfo.Id, model.MemberId, model.To);
                    break;
                case "folder-folder": //桌面应用从文件夹移动到另一个文件夹中
                    await _deskServices.FolderToOtherFolderAsync(UserInfo.Id, model.MemberId, model.To);
                    break;
                case "desk-desk": //桌面移动到另一个桌面
                    await _deskServices.DeskToOtherDeskAsync(UserInfo.Id, model.MemberId, model.To);
                    break;
                case "dock-otherdesk"://应用码头移动到另一个桌面
                    await _deskServices.DockToOtherDeskAsync(UserInfo.Id, model.MemberId, model.To);
                    break;
            }

            return Json(new ResponseModel
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
            Parameter.Validate(forms);
            #endregion
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

            await _deskServices.ModifyMemberInfoAsync(UserInfo.Id, memberDto);
            return Json(new ResponseModel
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
            var result = await _wallpaperServices.GetUploadWallpaperAsync(UserInfo.Id);
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
            var result = await _deskServices.GetAllSkinAsync(Appsetting.Skin);
            return Json(new ResponseModel<dynamic>
            {
                IsSuccess = true,
                Message = "获取皮肤列表成功",
                Model = new { result, currentSkin = (await _userServices.GetConfigAsync(UserInfo.Id)).Skin }
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
            Parameter.Validate(model.Pos);
            Parameter.Validate(model.DeskNum);
            #endregion

            await _deskServices.ModifyDockPositionAsync(UserInfo.Id, model.DeskNum, model.Pos);
            return Json(new ResponseModel
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
            Parameter.Validate(source);
            #endregion

            await _deskServices.ModifyWallpaperSourceAsync(source, UserInfo.Id);
            return Json(new ResponseModel
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
            Parameter.Validate(appSize);
            #endregion

            await _appServices.ModifyAppIconSizeAsync(UserInfo.Id, appSize);
            return Json(new ResponseModel
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
            var result = (await _userServices.GetConfigAsync(UserInfo.Id)).UserFace;
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
            Parameter.Validate(model.Name);
            Parameter.Validate(model.Icon);
            Parameter.Validate(model.MemberId);
            #endregion

            await _deskServices.ModifyFolderInfoAsync(UserInfo.Id, model.Name, model.Icon, model.MemberId);
            return Json(new ResponseModel
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
            var result = (await _userServices.GetConfigAsync(UserInfo.Id)).DockPosition;

            return Json(new ResponseModel<DockPosition>
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
            Parameter.Validate(deskNum);
            #endregion

            await _deskServices.ModifyDefaultDeskNumberAsync(UserInfo.Id, deskNum);
            return Json(new ResponseModel
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
            Parameter.Validate(appXy);
            #endregion

            await _appServices.ModifyAppDirectionAsync(UserInfo.Id, appXy);
            return Json(new ResponseModel
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
            Parameter.Validate(wallPaperShowType);
            #endregion

            await _wallpaperServices.ModifyWallpaperModeAsync(UserInfo.Id, wallPaperShowType);
            return Json(new ResponseModel
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
            var result = await _deskServices.GetDeskMembersAsync(UserInfo.Id);
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
            Parameter.Validate(appHorizontal);
            #endregion

            await _appServices.ModifyAppHorizontalSpacingAsync(UserInfo.Id, appHorizontal);
            return Json(new ResponseModel
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
            Parameter.Validate(appVertical);
            #endregion

            await _appServices.ModifyAppVerticalSpacingAsync(UserInfo.Id, appVertical);
            return Json(new ResponseModel
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
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);
            #endregion

            var result = await _deskServices.CheckUnreadNotifyCount(UserInfo.Id, pageIndex, pageSize);
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
            Parameter.Validate(notifyId, true);
            #endregion

            var response = new ResponseModel();

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