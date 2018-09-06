using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewCrmCore.Web.Controllers.ControllerHelper;
using NewCrmCore.Web.Filter;
using NewLibCore.Validate;
using NewLibCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace NewCrmCore.Web.Controllers
{
    public class DeskController : BaseController
    {
        private readonly IWallpaperServices _wallpaperServices;

        private readonly ISkinServices _skinServices;

        private readonly IDeskServices _deskServices;

        private readonly IAppServices _appServices;

        private readonly IUserServices _userServices;

        public DeskController(IWallpaperServices wallpaperServices,
        ISkinServices skinServices,
        IDeskServices deskServices,
        IAppServices appServices,
        IUserServices userServices)
        {
            _wallpaperServices = wallpaperServices;
            _skinServices = skinServices;
            _deskServices = deskServices;
            _appServices = appServices;
            _userServices = userServices;
        }

        #region 页面

        /// <summary>
        /// 桌面
        /// </summary>
        /// <returns></returns>
        [HttpGet, DoNotCheckPermission]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "桌面";
            if (HttpContext.Request.Cookies["User"] != null)
            {
                var user = await _userServices.GetUserAsync(UserId);
                ViewData["User"] = user;

                var config = await _userServices.GetConfigAsync(user.Id);
                if (config.IsModifyUserFace)
                {
                    user.UserFace = Appsetting.FileUrl + user.UserFace;
                }

                ViewData["UserConfig"] = config;
                ViewData["Desks"] = config.DefaultDeskCount;
                return View(user);
            }

            return RedirectToAction("login", "desk");
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        [HttpGet, DoNotCheckPermission]
        public IActionResult Login()
        {
            var userId = Request.Cookies["User"];
            if (userId != null)
            {
                return RedirectToAction("Index", "Desk");
            }

            return View();
        }

        /// <summary>
        /// 配置用户应用
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfigMember(Int32 memberId)
        {
            #region 参数验证
            Parameter.Validate(memberId);
            #endregion

            var result = await _deskServices.GetMemberAsync(UserId, memberId);
            return View(result);
        }

        /// <summary>
        /// 系统壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SystemWallPaper()
        {
            ViewData["UserConfig"] = await _userServices.GetConfigAsync(UserId);
            ViewData["Wallpapers"] = await _wallpaperServices.GetWallpapersAsync();

            return View();
        }

        /// <summary>
        /// 自定义壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CustomizeWallpaper()
        {
            ViewData["UserConfig"] = await _userServices.GetConfigAsync(UserId);
            return View();
        }

        /// <summary>
        /// 设置皮肤
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ConfigSkin()
        {
            return View();
        }

        /// <summary>
        /// 桌面设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfigDesk()
        {
            ViewData["UserConfig"] = await _userServices.GetConfigAsync(UserId);
            ViewData["Desks"] = (await _userServices.GetConfigAsync(UserId)).DefaultDeskCount;

            return View();
        }


        #endregion

        #region 登陆

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="loginParameter"></param>
        /// <returns></returns>
        [HttpPost, DoNotCheckPermission]
        public async Task<IActionResult> Landing(IFormCollection loginParameter)
        {
            #region 参数验证
            Parameter.Validate(loginParameter);
            #endregion

            var response = new ResponseModel<UserDto>();

            var user = await _userServices.LoginAsync(loginParameter["username"], loginParameter["password"], Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (user != null)
            {
                var cookieTimeout = (String.IsNullOrEmpty(loginParameter["remerber"]) ? false : Boolean.Parse(loginParameter["remerber"])) ? DateTime.Now.AddDays(7) : DateTime.Now.AddMinutes(60);
                response.Message = "登陆成功";
                response.IsSuccess = true;

                HttpContext.Response.Cookies.Append($@"User", JsonConvert.SerializeObject(new UserDto { Id = user.Id, UserFace = Appsetting.FileUrl + user.UserFace, Name = user.Name }), new CookieOptions { Expires = cookieTimeout });
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
        public async Task<IActionResult> ModifyWallpaper(Int32 wallpaperId)
        {
            #region 参数验证
            Parameter.Validate(wallpaperId);
            #endregion

            var response = new ResponseModel();
            await _wallpaperServices.ModifyWallpaperAsync(UserId, wallpaperId);
            response.IsSuccess = true;
            response.Message = "设置壁纸成功";

            return Json(response);
        }

        #endregion

        #region 删除壁纸

        /// <summary>
        /// 删除壁纸
        /// </summary>
        /// <param name="wallPaperId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveWallpaper(Int32 wallPaperId)
        {
            #region 参数验证
            Parameter.Validate(wallPaperId);
            #endregion

            var response = new ResponseModel<IList<WallpaperDto>>();
            await _wallpaperServices.RemoveWallpaperAsync(UserId, wallPaperId);
            response.IsSuccess = true;
            response.Message = "删除壁纸成功";

            return Json(response);
        }

        #endregion

        #region 上传壁纸

        /// <summary>
        /// 上传壁纸
        /// </summary>
        /// <param name="wallpaper"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadWallPaper(WallpaperDto wallpaper)
        {
            #region 参数验证
            Parameter.Validate(wallpaper);
            #endregion

            var response = new ResponseModel<dynamic>();

            var wallpaperResult = await _wallpaperServices.AddWallpaperAsync(new WallpaperDto
            {
                Title = wallpaper.Title.Substring(0, 9),
                Width = wallpaper.Width,
                Height = wallpaper.Height,
                Url = wallpaper.Url,
                Source = wallpaper.Source,
                UserId = UserId,
                Md5 = wallpaper.Md5,
                ShortUrl = ""
            });

            response.Message = "壁纸上传成功";
            response.IsSuccess = true;
            response.Model = new { Id = wallpaperResult.Item1, Url = Appsetting.FileUrl + wallpaperResult.Item2 };
            return Json(response);
        }

        #endregion

        #region 网络壁纸

        /// <summary>
        /// 网络壁纸
        /// </summary>
        /// <param name="webUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> WebWallpaper(String webUrl)
        {
            #region 参数验证
            Parameter.Validate(webUrl);
            #endregion

            var response = new ResponseModel<Tuple<Int32, String>>();

            var result = _wallpaperServices.AddWebWallpaperAsync(UserId, webUrl);
            response.IsSuccess = true;
            response.Message = "网络壁纸保存成功";
            response.Model = await result;

            return Json(response);
        }

        #endregion

        #region 更换皮肤

        /// <summary>
        /// 更换皮肤
        /// </summary>
        /// <param name="skin"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifySkin(String skin)
        {
            #region 参数验证
            Parameter.Validate(skin);
            #endregion

            var response = new ResponseModel();

            await _skinServices.ModifySkinAsync(UserId, skin);
            response.IsSuccess = true;
            response.Message = "更换皮肤成功";

            return Json(response);
        }

        #endregion

        #region 更新图标

        /// <summary>
        /// 更新图标
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyIcon(ModifyIconForMember model)
        {
            #region 参数验证
            Parameter.Validate(model.MemberId);
            Parameter.Validate(model.NewIcon);
            #endregion

            var response = new ResponseModel<String>();
            await _deskServices.ModifyMemberIconAsync(UserId, model.MemberId, model.NewIcon);

            response.IsSuccess = true;
            response.Message = "更新图标成功";
            response.Model = Appsetting.FileUrl + model.NewIcon;

            return Json(response);
        }

        #endregion

        #region 解锁屏幕

        /// <summary>
        /// 解锁屏幕
        /// </summary>
        /// <param name="unlockPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnlockScreen(String unlockPassword)
        {
            #region 参数验证
            Parameter.Validate(unlockPassword);
            #endregion

            var response = new ResponseModel();
            var result = await _userServices.UnlockScreenAsync(UserId, unlockPassword);
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
        public async Task<IActionResult> Logout()
        {
            await _userServices.LogoutAsync(UserId);
            Response.Cookies.Append("User", UserId.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            return new EmptyResult();
        }

        #endregion

        #region 获取皮肤

        /// <summary>
        /// 获取皮肤
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSkin()
        {
            var response = new ResponseModel<String>();
            var skinName = (await _userServices.GetConfigAsync(UserId)).Skin;
            response.IsSuccess = true;
            response.Model = skinName;
            response.Message = "初始化皮肤成功";

            return Json(response);
        }

        #endregion

        #region 获取壁纸

        /// <summary>
        /// 获取壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetWallpaper()
        {
            var response = new ResponseModel<ConfigDto>();
            var result = await _userServices.GetConfigAsync(UserId);

            if (result.WallpaperSource == WallpaperSource.Upload)
            {
                result.WallpaperUrl = Appsetting.FileUrl + result.WallpaperUrl;
            }

            if (result.IsBing)
            {
                result.WallpaperSource = WallpaperSource.Bing;
                result.WallpaperUrl = await BingHelper.GetEverydayBackgroundImageAsync();
            }

            response.IsSuccess = true;
            response.Message = "初始化壁纸成功";
            response.Model = result;

            return Json(response);
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
        public async Task<IActionResult> CreateWindow(Int32 id, String type)
        {

            #region 参数验证
            Parameter.Validate(id);
            Parameter.Validate(type);
            #endregion

            var response = new ResponseModel<dynamic>();
            var internalMemberResult = type == "folder" ? await _deskServices.GetMemberAsync(UserId, id, true) : await _deskServices.GetMemberAsync(UserId, id);
            response.IsSuccess = true;
            response.Message = "创建一个窗口成功";
            response.Model = new
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
        public async Task<IActionResult> CreateFolder(CreateFolder model)
        {
            #region 参数验证
            Parameter.Validate(model.FolderName);
            Parameter.Validate(model.FolderImg);
            Parameter.Validate(model.DeskId);
            #endregion

            var response = new ResponseModel();
            await _deskServices.CreateNewFolderAsync(model.FolderName, model.FolderImg, model.DeskId, UserId);
            response.IsSuccess = true;
            response.Message = "新建文件夹成功";

            return Json(response);
        }

        #endregion

        #region 卸载桌面成员

        /// <summary>
        /// 卸载桌面成员
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Uninstall(Int32 memberId)
        {
            #region 参数验证 
            Parameter.Validate(memberId);
            #endregion

            var response = new ResponseModel();
            await _deskServices.UninstallMemberAsync(UserId, memberId);
            response.IsSuccess = true;
            response.Message = "卸载成功";

            return Json(response);
        }

        #endregion

        #region 检查成员名称

        /// <summary>
        /// 检查成员名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CheckName(String param)
        {
            #region 参数验证
            Parameter.Validate(param);
            #endregion

            var result = await _deskServices.CheckMemberNameAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "应用名称已存在" });
        }

        #endregion

        #region 桌面成员移动

        /// <summary>
        /// 桌面成员移动
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MemberMove(MemberMove model)
        {
            #region 参数验证
            Parameter.Validate(model.MoveType);
            Parameter.Validate(model.MemberId);
            #endregion

            switch (model.MoveType.ToLower())
            {
                case "desk-dock": //成员从桌面移动到码头
                    await _deskServices.MemberInDockAsync(UserId, model.MemberId);
                    break;
                case "dock-desk": //成员从码头移动到桌面
                    await _deskServices.MemberOutDockAsync(UserId, model.MemberId, model.To);
                    break;
                case "dock-folder": //成员从码头移动到桌面文件夹中
                    await _deskServices.DockToFolderAsync(UserId, model.MemberId, model.To);
                    break;
                case "folder-dock": //成员从文件夹移动到码头
                    await _deskServices.FolderToDockAsync(UserId, model.MemberId);
                    break;
                case "desk-folder": //成员从桌面移动到文件夹
                    await _deskServices.DeskToFolderAsync(UserId, model.MemberId, model.To);
                    break;
                case "folder-desk": //成员从文件夹移动到桌面
                    await _deskServices.FolderToDeskAsync(UserId, model.MemberId, model.To);
                    break;
                case "folder-folder": //成员从文件夹移动到另一个文件夹中
                    await _deskServices.FolderToOtherFolderAsync(UserId, model.MemberId, model.To);
                    break;
                case "desk-desk": //桌面移动到另一个桌面
                    await _deskServices.DeskToOtherDeskAsync(UserId, model.MemberId, model.To);
                    break;
                case "dock-otherdesk"://应用码头移动到另一个桌面
                    await _deskServices.DockToOtherDeskAsync(UserId, model.MemberId, model.To);
                    break;
            }
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "移动成功"
            };

            return Json(response);
        }

        #endregion

        #region 修改成员信息

        /// <summary>
        /// 修改成员信息
        /// </summary>
        /// <param name="forms"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyMemberInfo(IFormCollection forms)
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

            var response = new ResponseModel();
            await _deskServices.ModifyMemberInfoAsync(UserId, memberDto);
            response.IsSuccess = true;
            response.Message = "修改成员信息成功";

            return Json(response);
        }

        #endregion

        #region 载入上传壁纸

        /// <summary>
        /// 载入上传壁纸
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUploadWallPapers()
        {
            var response = new ResponseModel<IList<WallpaperDto>>();
            var result = await _wallpaperServices.GetUploadWallpaperAsync(UserId);
            response.IsSuccess = true;
            response.Message = "载入之前上传的壁纸成功";
            response.Model = result;

            return Json(response);
        }

        #endregion

        #region 获取全部皮肤

        /// <summary>
        /// 获取全部皮肤
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSkins()
        {
            var response = new ResponseModel<dynamic>();

            var skinPath = Appsetting.Skin;

            var result = await _skinServices.GetAllSkinAsync(skinPath);
            response.IsSuccess = true;
            response.Message = "获取皮肤列表成功";
            response.Model = new { result, currentSkin = (await _userServices.GetConfigAsync(UserId)).Skin };
            return Json(response);

        }

        #endregion

        #region 更改码头位置

        /// <summary>
        /// 更改码头位置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyDockPosition(ModifyDockPosition model)
        {
            #region 参数验证
            Parameter.Validate(model.Pos);
            Parameter.Validate(model.DeskNum);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyDockPositionAsync(UserId, model.DeskNum, model.Pos);
            response.IsSuccess = true;
            response.Message = "更改码头的位置成功";

            return Json(response);
        }

        #endregion

        #region 修改壁纸来源

        /// <summary>
        /// 修改壁纸来源
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyWallpaperSource(String source)
        {
            #region 参数验证
            Parameter.Validate(source);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyWallpaperSourceAsync(source, UserId);
            response.IsSuccess = true;
            response.Message = "更改壁纸来源成功";

            return Json(response);
        }

        #endregion

        #region 更改图标大小

        /// <summary>
        /// 更改图标大小
        /// </summary>
        /// <param name="appSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifySize(Int32 appSize)
        {
            #region 参数验证
            Parameter.Validate(appSize);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyAppIconSizeAsync(UserId, appSize);
            response.IsSuccess = true;
            response.Message = "更改图标大小成功";

            return Json(response);
        }

        #endregion

        #region 获取账户头像

        /// <summary>
        /// 获取账户头像
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserFace()
        {
            var response = new ResponseModel<String>();
            var result = (await _userServices.GetConfigAsync(UserId)).UserFace;
            response.IsSuccess = true;
            response.Message = "获取用户头像成功";
            response.Model = Appsetting.FileUrl + result;

            return Json(response);
        }

        #endregion

        #region 修改文件夹信息

        /// <summary>
        /// 修改文件夹信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyFolderInfo(ModifyFolderInfo model)
        {
            #region 参数验证
            Parameter.Validate(model.Name);
            Parameter.Validate(model.Icon);
            Parameter.Validate(model.MemberId);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyFolderInfoAsync(UserId, model.Name, model.Icon, model.MemberId);
            response.IsSuccess = true;
            response.Message = "修改成功";

            return Json(response);
        }

        #endregion

        #region 初始化应用码头

        /// <summary>
        /// 初始化应用码头
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDockPos()
        {
            var response = new ResponseModel<DockPosition>();
            var result = (await _userServices.GetConfigAsync(UserId)).DockPosition;
            response.IsSuccess = true;
            response.Message = "初始化应用码头成功";
            response.Model = result;

            return Json(response);
        }

        #endregion

        #region 更换默认显示桌面

        /// <summary>
        /// 更换默认显示桌面
        /// </summary>
        /// <param name="deskNum"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyDefaultDesk(Int32 deskNum)
        {
            #region 参数验证
            Parameter.Validate(deskNum);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyDefaultDeskNumberAsync(UserId, deskNum);
            response.IsSuccess = true;
            response.Message = "更换默认桌面成功";

            return Json(response);
        }

        #endregion

        #region 更换图标排列方向

        /// <summary>
        /// 更换图标排列方向
        /// </summary>
        /// <param name="appXy"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyXy(String appXy)
        {
            #region 参数验证
            Parameter.Validate(appXy);
            #endregion

            var response = new ResponseModel();

            await _appServices.ModifyAppDirectionAsync(UserId, appXy);
            response.IsSuccess = true;
            response.Message = "更换图标排列方向成功";

            return Json(response);
        }

        #endregion

        #region 设置壁纸显示模式

        /// <summary>
        /// 设置壁纸显示模式
        /// </summary>
        /// <param name="wallPaperShowType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyDisplayModel(String wallPaperShowType)
        {
            #region 参数验证
            Parameter.Validate(wallPaperShowType);
            #endregion

            var response = new ResponseModel();
            await _wallpaperServices.ModifyWallpaperModeAsync(UserId, wallPaperShowType);
            response.IsSuccess = true;
            response.Message = "壁纸显示模式设置成功";

            return Json(response);
        }

        #endregion

        #region 获取账户安装的应用

        /// <summary>
        /// 获取账户安装的应用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserDeskMembers()
        {
            var response = new ResponseModel<IDictionary<String, IList<dynamic>>>();
            var result = await _deskServices.GetDeskMembersAsync(UserId);
            response.IsSuccess = true;
            response.Message = "获取我的应用成功";
            response.Model = result;

            return Json(response);
        }

        #endregion

        #region 更改图标的水平间距

        /// <summary>
        /// 更改图标的水平间距
        /// </summary>
        /// <param name="appHorizontal"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyHorizontalSpace(Int32 appHorizontal)
        {
            #region 参数验证
            Parameter.Validate(appHorizontal);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyAppHorizontalSpacingAsync(UserId, appHorizontal);
            response.IsSuccess = true;
            response.Message = "更改图标水平间距成功";

            return Json(response);
        }

        #endregion

        #region 更改应用图标垂直间距

        /// <summary>
        /// 更改应用图标垂直间距
        /// </summary>
        /// <param name="appVertical"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ModifyVerticalSpace(Int32 appVertical)
        {
            #region 参数验证
            Parameter.Validate(appVertical);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyAppVerticalSpacingAsync(UserId, appVertical);
            response.IsSuccess = true;
            response.Message = "更改图标垂直间距成功";

            return Json(response);
        }

        #endregion

        /// <summary>
        /// 检查未读通知数量
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckUnreadNotifyCount(Int32 pageIndex, Int32 pageSize)
        {
            #region 参数验证
            Parameter.Validate(pageIndex);
            Parameter.Validate(pageSize);
            #endregion

            var response = new ResponseModel<PageList<NotifyDto>>();
            response.IsSuccess = true;
            response.Model = await _deskServices.CheckUnreadNotifyCount(UserId, pageIndex, pageSize);
            response.Message = "消息列表获取成功";

            return Json(response);
        }

        /// <summary>
        /// 读取通知
        /// </summary>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadNotify(String notifyId)
        {
            #region 参数验证
            Parameter.Validate(notifyId, true);
            #endregion

            var response = new ResponseModel();

            if (String.IsNullOrEmpty(notifyId))
            {
                response.IsSuccess = true;
                response.Message = "消息读取成功";
                return Json(response);
            }
            await _deskServices.ReadNotify(notifyId.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList());
            response.IsSuccess = true;
            response.Message = "消息读取成功";

            return Json(response);
        }
    }
}