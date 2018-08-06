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

namespace NewCrmCore.Web.Controllers
{
    public class DeskController : BaseController
    {
        private readonly IWallpaperServices _wallpaperServices;
        private readonly ISkinServices _skinServices;
        private readonly IDeskServices _deskServices;
        private readonly IAppServices _appServices;
        private readonly IAccountServices _accountServices;

        public DeskController(IWallpaperServices wallpaperServices,
        ISkinServices skinServices,
        IDeskServices deskServices,
        IAppServices appServices,
        IAccountServices accountServices)
        {
            _wallpaperServices = wallpaperServices;
            _skinServices = skinServices;
            _deskServices = deskServices;
            _appServices = appServices;
            _accountServices = accountServices;
        }

        #region 页面

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet, DoNotCheckPermission]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "桌面";
            if (HttpContext.Request.Cookies["Account"] != null)
            {
                var account = await _accountServices.GetAccountAsync(AccountId);
                account.AccountFace = Appsetting.FileUrl + account.AccountFace;
                ViewData["Account"] = account;
                ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(account.Id);
                ViewData["Desks"] = (await _accountServices.GetConfigAsync(account.Id)).DefaultDeskCount;

                return View(account);
            }

            return RedirectToAction("Login", "Desk");
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet, DoNotCheckPermission]
        public IActionResult Login()
        {
            var accountId = Request.Cookies["Account"];
            if (accountId != null)
            {
                return RedirectToAction("Index", "Desk");
            }

            return View();
        }

        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfigMember(Int32 memberId)
        {
            #region 参数验证
            new Parameter().Validate(memberId);
            #endregion

            var result = await _deskServices.GetMemberAsync(AccountId, memberId);
            return View(result);
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SystemWallPaper()
        {
            ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(AccountId);
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
            ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(AccountId);
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
        /// 程序设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfigDesk()
        {
            ViewData["AccountConfig"] = await _accountServices.GetConfigAsync(AccountId);
            ViewData["Desks"] = (await _accountServices.GetConfigAsync(AccountId)).DefaultDeskCount;

            return View();
        }


        #endregion

        #region 登陆

        /// <summary>
        /// 登陆
        /// </summary>
        [HttpPost, DoNotCheckPermission]
        public async Task<IActionResult> Landing(IFormCollection loginParameter)
        {

            #region 参数验证
            new Parameter().Validate(loginParameter);
            #endregion

            var response = new ResponseModel<AccountDto>();

            var account = await _accountServices.LoginAsync(loginParameter["accountname"], loginParameter["password"], Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (account != null)
            {
                var cookieTimeout = (String.IsNullOrEmpty(loginParameter["remerber"]) ? false : Boolean.Parse(loginParameter["remerber"])) ? DateTime.Now.AddDays(7) : DateTime.Now.AddMinutes(60);
                response.Message = "登陆成功";
                response.IsSuccess = true;

                HttpContext.Response.Cookies.Append("Account", JsonConvert.SerializeObject(new AccountDto { Id = account.Id, AccountFace = Appsetting.FileUrl + account.AccountFace, Name = account.Name }), new CookieOptions { Expires = cookieTimeout });

            }
            return Json(response);
        }

        #endregion

        #region 设置壁纸

        /// <summary>
        /// 设置壁纸
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifyWallpaper([FromBody]Int32 wallpaperId)
        {
            #region 参数验证
            new Parameter().Validate(wallpaperId);
            #endregion

            var response = new ResponseModel();
            await _wallpaperServices.ModifyWallpaperAsync(AccountId, wallpaperId);
            response.IsSuccess = true;
            response.Message = "设置壁纸成功";

            return Json(response);
        }

        #endregion

        #region 删除壁纸

        /// <summary>
        /// 删除壁纸
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RemoveWallpaper([FromBody]Int32 wallPaperId)
        {
            #region 参数验证
            new Parameter().Validate(wallPaperId);
            #endregion

            var response = new ResponseModel<IList<WallpaperDto>>();
            await _wallpaperServices.RemoveWallpaperAsync(AccountId, wallPaperId);
            response.IsSuccess = true;
            response.Message = "删除壁纸成功";

            return Json(response);
        }

        #endregion

        #region 上传壁纸

        /// <summary>
        /// 上传壁纸     
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadWallPaper([FromBody]WallpaperDto wallpaper)
        {
            #region 参数验证
            new Parameter().Validate(wallpaper);
            #endregion

            var response = new ResponseModel<dynamic>();

            var wallpaperResult = await _wallpaperServices.AddWallpaperAsync(new WallpaperDto
            {
                Title = wallpaper.Title.Substring(0, 9),
                Width = wallpaper.Width,
                Height = wallpaper.Height,
                Url = wallpaper.Url,
                Source = wallpaper.Source,
                AccountId = AccountId,
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
        [HttpPost]
        public async Task<IActionResult> WebWallpaper([FromBody]String webUrl)
        {
            #region 参数验证
            new Parameter().Validate(webUrl);
            #endregion

            var response = new ResponseModel<Tuple<Int32, String>>();

            var result = _wallpaperServices.AddWebWallpaperAsync(AccountId, webUrl);
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
        [HttpPost]
        public async Task<IActionResult> ModifySkin([FromBody]String skin)
        {
            #region 参数验证
            new Parameter().Validate(skin);
            #endregion

            var response = new ResponseModel();

            await _skinServices.ModifySkinAsync(AccountId, skin);
            response.IsSuccess = true;
            response.Message = "更换皮肤成功";

            return Json(response);
        }

        #endregion

        #region 更新图标

        /// <summary>
        /// 更新图标
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifyIcon([FromBody]ModifyIconForMember model)
        {
            #region 参数验证
            new Parameter().Validate(model.MemberId).Validate(model.NewIcon);
            #endregion

            var response = new ResponseModel<String>();
            await _deskServices.ModifyMemberIconAsync(AccountId, model.MemberId, model.NewIcon);

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
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnlockScreen([FromBody]String unlockPassword)
        {
            #region 参数验证
            new Parameter().Validate(unlockPassword);
            #endregion

            var response = new ResponseModel();
            var result = await _accountServices.UnlockScreenAsync(AccountId, unlockPassword);
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
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountServices.LogoutAsync(AccountId);
            Response.Cookies.Append("Account", AccountId.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            return new EmptyResult();
        }

        #endregion

        #region 获取皮肤

        /// <summary>
        /// 获取皮肤
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSkin()
        {
            var response = new ResponseModel<String>();
            var skinName = (await _accountServices.GetConfigAsync(AccountId)).Skin;
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
        [HttpGet]
        public async Task<IActionResult> GetWallpaper()
        {
            var response = new ResponseModel<ConfigDto>();
            var result = await _accountServices.GetConfigAsync(AccountId);

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
        [HttpGet]
        public async Task<IActionResult> CreateWindow(Int32 id, String type)
        {

            #region 参数验证
            new Parameter().Validate(id).Validate(type);
            #endregion

            var response = new ResponseModel<dynamic>();
            var internalMemberResult = type == "folder" ? await _deskServices.GetMemberAsync(AccountId, id, true) : await _deskServices.GetMemberAsync(AccountId, id);
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
                isDraw = internalMemberResult.IsDraw,
                isOpenMax = internalMemberResult.IsOpenMax,
                isSetbar = internalMemberResult.IsSetbar,
                url = internalMemberResult.AppUrl,
                isResize = internalMemberResult.IsResize
            };

            return Json(response);
        }

        #endregion

        #region 新建文件夹

        /// <summary>
        /// 新建文件夹
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody]CreateFolder model)
        {
            #region 参数验证
            new Parameter().Validate(model.FolderName).Validate(model.FolderImg).Validate(model.DeskId);
            #endregion

            var response = new ResponseModel();
            await _deskServices.CreateNewFolderAsync(model.FolderName, model.FolderImg, model.DeskId, AccountId);
            response.IsSuccess = true;
            response.Message = "新建文件夹成功";

            return Json(response);
        }

        #endregion

        #region 卸载桌面成员

        /// <summary>
        /// 卸载桌面成员
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Uninstall([FromBody]Int32 memberId)
        {
            #region 参数验证
            new Parameter().Validate(memberId);
            #endregion

            var response = new ResponseModel();
            await _deskServices.UninstallMemberAsync(AccountId, memberId);
            response.IsSuccess = true;
            response.Message = "卸载成功";

            return Json(response);
        }

        #endregion

        #region 检查成员名称

        /// <summary>
        /// 检查成员名称
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckName([FromBody]String param)
        {
            #region 参数验证
            new Parameter().Validate(param);
            #endregion

            var result = await _deskServices.CheckMemberNameAsync(param);
            return Json(!result ? new { status = "y", info = "" } : new { status = "n", info = "应用名称已存在" });
        }

        #endregion

        #region 桌面成员移动

        /// <summary>
        /// 桌面成员移动
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MemberMove([FromBody]MemberMove model)
        {
            #region 参数验证
            new Parameter().Validate(model.MoveType).Validate(model.MemberId);
            #endregion

            switch (model.MoveType.ToLower())
            {
                case "desk-dock": //成员从桌面移动到码头
                    await _deskServices.MemberInDockAsync(AccountId, model.MemberId);
                    break;
                case "dock-desk": //成员从码头移动到桌面
                    await _deskServices.MemberOutDockAsync(AccountId, model.MemberId, model.To);
                    break;
                case "dock-folder": //成员从码头移动到桌面文件夹中
                    await _deskServices.DockToFolderAsync(AccountId, model.MemberId, model.To);
                    break;
                case "folder-dock": //成员从文件夹移动到码头
                    await _deskServices.FolderToDockAsync(AccountId, model.MemberId);
                    break;
                case "desk-folder": //成员从桌面移动到文件夹
                    await _deskServices.DeskToFolderAsync(AccountId, model.MemberId, model.To);
                    break;
                case "folder-desk": //成员从文件夹移动到桌面
                    await _deskServices.FolderToDeskAsync(AccountId, model.MemberId, model.To);
                    break;
                case "folder-folder": //成员从文件夹移动到另一个文件夹中
                    await _deskServices.FolderToOtherFolderAsync(AccountId, model.MemberId, model.To);
                    break;
                case "desk-desk": //桌面移动到另一个桌面
                    await _deskServices.DeskToOtherDeskAsync(AccountId, model.MemberId, model.To);
                    break;
                case "dock-otherdesk"://应用码头移动到另一个桌面
                    await _deskServices.DockToOtherDeskAsync(AccountId, model.MemberId, model.To);
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
        [HttpPost]
        public async Task<IActionResult> ModifyMemberInfo(IFormCollection forms)
        {
            #region 参数验证
            new Parameter().Validate(forms);
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
            await _deskServices.ModifyMemberInfoAsync(AccountId, memberDto);
            response.IsSuccess = true;
            response.Message = "修改成员信息成功";

            return Json(response);
        }

        #endregion

        #region 载入上传壁纸

        /// <summary>
        /// 载入上传壁纸
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUploadWallPapers()
        {
            var response = new ResponseModel<IList<WallpaperDto>>();
            var result = await _wallpaperServices.GetUploadWallpaperAsync(AccountId);
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
        [HttpGet]
        public async Task<IActionResult> GetSkins()
        {
            var response = new ResponseModel<dynamic>();

            var skinPath = Appsetting.Skin;

            var result = await _skinServices.GetAllSkinAsync(skinPath);
            response.IsSuccess = true;
            response.Message = "获取皮肤列表成功";
            response.Model = new { result, currentSkin = (await _accountServices.GetConfigAsync(AccountId)).Skin };
            return Json(response);

        }

        #endregion

        #region 更改码头位置

        /// <summary>
        /// 更改码头位置
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifyDockPosition([FromBody] ModifyDockPosition model)
        {
            #region 参数验证
            new Parameter().Validate(model.Pos).Validate(model.DeskNum);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyDockPositionAsync(AccountId, model.DeskNum, model.Pos);
            response.IsSuccess = true;
            response.Message = "更改码头的位置成功";

            return Json(response);
        }

        #endregion

        #region 修改壁纸来源

        /// <summary>
        /// 修改壁纸来源
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifyWallpaperSource([FromBody]String source)
        {
            #region 参数验证
            new Parameter().Validate(source);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyWallpaperSourceAsync(source, AccountId);
            response.IsSuccess = true;
            response.Message = "更改壁纸来源成功";

            return Json(response);
        }

        #endregion

        #region 更改图标大小

        /// <summary>
        /// 更改图标大小
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifySize([FromBody]Int32 appSize)
        {
            #region 参数验证
            new Parameter().Validate(appSize);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyAppIconSizeAsync(AccountId, appSize);
            response.IsSuccess = true;
            response.Message = "更改图标大小成功";

            return Json(response);
        }

        #endregion

        #region 获取账户头像

        /// <summary>
        /// 获取账户头像
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAccountFace()
        {
            var response = new ResponseModel<String>();
            var result = (await _accountServices.GetConfigAsync(AccountId)).AccountFace;
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
        [HttpPost]
        public async Task<IActionResult> ModifyFolderInfo([FromBody]ModifyFolderInfo model)
        {
            #region 参数验证
            new Parameter().Validate(model.Name).Validate(model.Icon).Validate(model.MemberId);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyFolderInfoAsync(AccountId, model.Name, model.Icon, model.MemberId);
            response.IsSuccess = true;
            response.Message = "修改成功";

            return Json(response);
        }

        #endregion

        #region 初始化应用码头

        /// <summary>
        /// 初始化应用码头
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDockPos()
        {
            var response = new ResponseModel<DockPosition>();
            var result = (await _accountServices.GetConfigAsync(AccountId)).DockPosition;
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
        [HttpPost]
        public async Task<IActionResult> ModifyDefaultDesk([FromBody]Int32 deskNum)
        {
            #region 参数验证
            new Parameter().Validate(deskNum);
            #endregion

            var response = new ResponseModel();
            await _deskServices.ModifyDefaultDeskNumberAsync(AccountId, deskNum);
            response.IsSuccess = true;
            response.Message = "更换默认桌面成功";

            return Json(response);
        }

        #endregion

        #region 更换图标排列方向

        /// <summary>
        /// 更换图标排列方向
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifyXy([FromBody]String appXy)
        {
            #region 参数验证
            new Parameter().Validate(appXy);
            #endregion

            var response = new ResponseModel();

            await _appServices.ModifyAppDirectionAsync(AccountId, appXy);
            response.IsSuccess = true;
            response.Message = "更换图标排列方向成功";

            return Json(response);
        }

        #endregion

        #region 设置壁纸显示模式

        /// <summary>
        /// 设置壁纸显示模式
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifyDisplayModel([FromBody]String wallPaperShowType)
        {
            #region 参数验证
            new Parameter().Validate(wallPaperShowType);
            #endregion

            var response = new ResponseModel();
            await _wallpaperServices.ModifyWallpaperModeAsync(AccountId, wallPaperShowType);
            response.IsSuccess = true;
            response.Message = "壁纸显示模式设置成功";

            return Json(response);
        }

        #endregion

        #region 获取账户安装的应用

        /// <summary>
        /// 获取账户安装的应用
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAccountDeskMembers()
        {
            var response = new ResponseModel<IDictionary<String, IList<dynamic>>>();
            var result = await _deskServices.GetDeskMembersAsync(AccountId);
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
        [HttpPost]
        public async Task<IActionResult> ModifyHorizontalSpace([FromBody]Int32 appHorizontal)
        {
            #region 参数验证
            new Parameter().Validate(appHorizontal);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyAppHorizontalSpacingAsync(AccountId, appHorizontal);
            response.IsSuccess = true;
            response.Message = "更改图标水平间距成功";

            return Json(response);
        }

        #endregion

        #region 更改应用图标垂直间距

        /// <summary>
        /// 更改应用图标垂直间距
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ModifyVerticalSpace([FromBody]Int32 appVertical)
        {
            #region 参数验证
            new Parameter().Validate(appVertical);
            #endregion

            var response = new ResponseModel();
            await _appServices.ModifyAppVerticalSpacingAsync(AccountId, appVertical);
            response.IsSuccess = true;
            response.Message = "更改图标垂直间距成功";

            return Json(response);
        }

        #endregion

    }
}