using System;
using System.Linq;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Services.Interface;
using NewCrmCore.Domain.ValueObject;
using NewCrmCore.Dto;
using NewCrmCore.Infrastructure;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
using NewLibCore.Security;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserContext _userContext;

        public UserServices(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public async Task<UserDto> LoginAsync(String userName, String password, String requestIp)
        {
            Parameter.Validate(userName);
            Parameter.Validate(password);
            Parameter.Validate(requestIp);

            var user = await _userContext.ValidateAsync(userName, password, requestIp);
            return new UserDto
            {
                Name = user.Name,
                Id = user.Id,
                UserFace = user.IsModifyUserFace ? Appsetting.FileUrl + user.UserFace : user.UserFace,
                IsAdmin = user.IsAdmin,
                Password = user.LoginPassword,
                LockScreenPassword = user.LockScreenPassword
            };
        }

        public async Task<ConfigDto> GetConfigAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            var config = await CacheHelper.GetOrSetCacheAsync(new ConfigCacheKey(userId), () => _userContext.GetConfigAsync(userId));
            if (config == null)
            {
                throw new BusinessException("桌面配置信息获取失败");
            }
            var configDto = new ConfigDto
            {
                Id = config.Id,
                Skin = config.Skin,
                UserFace = config.UserFace,
                AppSize = config.AppSize,
                AppVerticalSpacing = config.AppVerticalSpacing,
                AppHorizontalSpacing = config.AppHorizontalSpacing,
                DefaultDeskNumber = config.DefaultDeskNumber,
                DefaultDeskCount = config.DefaultDeskCount,
                IsModifyUserFace = config.IsModifyUserFace,
                AppXy = config.AppXy,
                DockPosition = config.DockPosition,
                WallpaperMode = config.WallpaperMode,
                IsBing = config.IsBing,
                UserId = config.UserId
            };

            if (!config.IsBing)
            {
                var wallpaper = await CacheHelper.GetOrSetCacheAsync(new WallpaperCacheKey(userId), () => _userContext.GetWallpaperAsync(config.WallpaperId));
                configDto.WallpaperUrl = wallpaper.Url;
                configDto.WallpaperWidth = wallpaper.Width;
                configDto.WallpaperHeigth = wallpaper.Height;
                configDto.WallpaperSource = wallpaper.Source;
            }

            return configDto;
        }

        public async Task<PageList<UserDto>> GetUsersAsync(String userName, String userType, Int32 pageIndex, Int32 pageSize)
        {
            return await Task.Run(() =>
            {
                var result = _userContext.GetUsers(userName, userType, pageIndex, pageSize, out var totalCount);
                var pagingModel = new PageList<UserDto>();
                pagingModel.TotalCount = totalCount;
                pagingModel.Models = result.Select(s => new UserDto
                {
                    Id = s.Id,
                    IsAdmin = s.IsAdmin,
                    Name = s.Name,
                    UserFace = s.IsModifyUserFace ? Appsetting.FileUrl + s.UserFace : s.UserFace,
                    IsDisable = s.IsDisable
                }).ToList();

                return pagingModel;
            });
        }

        public async Task<UserDto> GetUserAsync(Int32 userId)
        {
            Parameter.Validate(userId);

            var user = await CacheHelper.GetOrSetCacheAsync(new UserCacheKey(userId), () => _userContext.GetUserAsync(userId));
            if (user == null)
            {
                throw new BusinessException("该用户可能已被禁用或被删除，请联系管理员");
            }

            var roles = await CacheHelper.GetOrSetCacheAsync(new UserRoleCacheKey(user.Id), () => _userContext.GetRolesAsync(user.Id));
            var powers = await CacheHelper.GetOrSetCacheAsync(new PowersCacheKey(user.Id), () => _userContext.GetPowersAsync());

            return new UserDto
            {
                UserFace = user.IsModifyUserFace ? Appsetting.FileUrl + user.UserFace : user.UserFace,
                AddTime = user.AddTime.ToString("yyyy-MM-dd"),
                Id = user.Id,
                IsAdmin = user.IsAdmin,
                IsDisable = user.IsDisable,
                IsOnline = user.IsOnline,
                LastLoginTime = user.LastLoginTime.ToString("yyyy-MM-dd HH:mm"),
                LockScreenPassword = user.LockScreenPassword,
                Password = user.LoginPassword,
                Name = user.Name,
                Roles = roles.Select(s => new RoleDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Powers = powers.Where(w => w.RoleId == s.Id).Select(p => new PowerDto
                    {
                        Id = p.AppId
                    }).ToList(),
                    RoleIdentity = s.RoleIdentity
                }).ToList()
            };
        }

        public async Task<Boolean> CheckUserNameExistAsync(String userName)
        {
            Parameter.Validate(userName);
            return await _userContext.CheckUserNameExistAsync(userName);
        }

        public async Task<Boolean> CheckPasswordAsync(Int32 userId, String oldUserPassword)
        {
            Parameter.Validate(userId);
            Parameter.Validate(oldUserPassword);
            var result = await _userContext.GetOldPasswordAsync(userId);
            return PasswordUtil.ComparePasswords(result, oldUserPassword);
        }

        public async Task<Boolean> UnlockScreenAsync(Int32 userId, String unlockPassword)
        {
            Parameter.Validate(userId);
            Parameter.Validate(unlockPassword);
            return await _userContext.UnlockScreenAsync(userId, unlockPassword);
        }

        public async Task<Boolean> CheckAppNameAsync(String name)
        {
            Parameter.Validate(name);
            return await _userContext.CheckAppNameAsync(name);
        }

        public async Task<Boolean> CheckAppUrlAsync(String url)
        {
            Parameter.Validate(url);
            return await _userContext.CheckAppUrlAsync(url);
        }

        public async Task AddNewUserAsync(UserDto userDto)
        {
            Parameter.Validate(userDto);

            var user = userDto.ConvertToModel<UserDto, User>();

            var userType = EnumExtensions.ToEnum<UserType>(user.IsAdmin ? 2 /*管理员*/ : 1 /*用户*/);
            var newPassword = PasswordUtil.CreateDbPassword(user.LoginPassword);

            var internalNewUser = new User(user.Name, newPassword, user.Roles, userType);
            await _userContext.AddNewUserAsync(internalNewUser);
        }

        public async Task ModifyUserAsync(UserDto userDto)
        {
            Parameter.Validate(userDto);

            var user = userDto.ConvertToModel<UserDto, User>();
            await _userContext.ModifyUserAsync(user);
            await CacheHelper.RemoveKeyWhenModify(new UserCacheKey(user.Id), new UserRoleCacheKey(user.Id));
        }

        public async Task LogoutAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            await _userContext.LogoutAsync(userId);
            await CacheHelper.RemoveKeyWhenModify(new UserCacheKey(userId));
        }

        public async Task EnableAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            await _userContext.EnableAsync(userId);
        }

        public async Task DisableAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            await _userContext.DisableAsync(userId);
        }

        public async Task ModifyUserFaceAsync(Int32 userId, String newFace)
        {
            Parameter.Validate(userId);
            Parameter.Validate(newFace);

            await _userContext.ModifyUserFaceAsync(userId, newFace);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId), new UserCacheKey(userId));
        }

        public async Task ModifyPasswordAsync(Int32 userId, String newPassword, Boolean isTogetherSetLockPassword)
        {
            Parameter.Validate(newPassword);

            var password = PasswordUtil.CreateDbPassword(newPassword);
            await _userContext.ModifyPasswordAsync(userId, password, isTogetherSetLockPassword);
        }

        public async Task ModifyLockScreenPasswordAsync(Int32 userId, String newScreenPassword)
        {
            Parameter.Validate(newScreenPassword);

            var newPassword = PasswordUtil.CreateDbPassword(newScreenPassword);
            await _userContext.ModifyLockScreenPasswordAsync(userId, newPassword);
            await CacheHelper.RemoveKeyWhenModify(new ConfigCacheKey(userId));
        }

        public async Task RemoveUserAsync(Int32 userId)
        {
            Parameter.Validate(userId);
            await _userContext.RemoveUserAsync(userId);
        }
    }
}
