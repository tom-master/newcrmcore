using System;
using AutoMapper;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Validate;

namespace NewCrmCore.Dto.MapperProfile
{
    internal class UserDtoToUserProfile : Profile
    {
        public UserDtoToUserProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(user => user.Name, dto => dto.MapFrom(d => d.Name))
                .ForMember(user => user.Id, dto => dto.MapFrom(d => d.Id))
                .ForMember(user => user.LoginPassword, dto => dto.MapFrom(d => d.Password))
                .ForMember(user => user.IsAdmin, dto => dto.MapFrom(d => d.IsAdmin))
                .ForMember(user => user.Roles, dto => dto.MapFrom(d => d.Roles))
                .ForMember(dto => dto.IsDisable, user => user.MapFrom(u => u.IsDisable));
        }
    }

    internal class WallpaperDtoToWallpaperProfile : Profile
    {
        public WallpaperDtoToWallpaperProfile()
        {
            CreateMap<WallpaperDto, Wallpaper>()
                .ForMember(wallpaper => wallpaper.Id, dto => dto.MapFrom(d => d.Id))
                .ForMember(wallpaper => wallpaper.Height, dto => dto.MapFrom(d => d.Height))
                .ForMember(wallpaper => wallpaper.Source, dto => dto.MapFrom(d => d.Source))
                .ForMember(wallpaper => wallpaper.Title, dto => dto.MapFrom(d => d.Title))
                .ForMember(wallpaper => wallpaper.Url, dto => dto.MapFrom(d => d.Url))
                .ForMember(wallpaper => wallpaper.Width, dto => dto.MapFrom(d => d.Width))
                .ForMember(wallpaper => wallpaper.ShortUrl, dto => dto.MapFrom(d => d.ShortUrl))
                .ForMember(wallpaper => wallpaper.Md5, dto => dto.MapFrom(d => d.Md5));
        }
    }

    internal class MemberDtoToMemberProfile : Profile
    {
        public MemberDtoToMemberProfile()
        {
            CreateMap<MemberDto, Member>()
             .ForMember(member => member.Id, dto => dto.MapFrom(w => w.Id))
             .ForMember(member => member.AppId, dto => dto.MapFrom(w => w.AppId))
             .ForMember(member => member.Width, dto => dto.MapFrom(w => w.Width))
             .ForMember(member => member.Height, dto => dto.MapFrom(w => w.Height))
             .ForMember(member => member.FolderId, dto => dto.MapFrom(w => w.FolderId))
             .ForMember(member => member.Name, dto => dto.MapFrom(w => w.Name))
             .ForMember(member => member.IconUrl, dto => dto.MapFrom(w => w.IconUrl))
             .ForMember(member => member.MemberType, dto => dto.MapFrom(w => w.MemberType))
             .ForMember(member => member.IsOnDock, dto => dto.MapFrom(w => w.IsOnDock))
             .ForMember(member => member.IsSetbar, dto => dto.MapFrom(w => w.IsSetbar))
             .ForMember(member => member.IsOpenMax, dto => dto.MapFrom(w => w.IsOpenMax))
             .ForMember(member => member.IsFlash, dto => dto.MapFrom(w => w.IsFlash))
             .ForMember(member => member.IsResize, dto => dto.MapFrom(w => w.IsResize))
             .ForMember(member => member.AppUrl, dto => dto.MapFrom(w => w.AppUrl))
             .ForMember(member => member.DeskIndex, dto => dto.MapFrom(w => w.DeskIndex))
             .ForMember(member => member.UserId, dto => dto.MapFrom(w => w.UserId))
             .ForMember(member => member.IsIconByUpload, dto => dto.MapFrom(w => w.IsIconByUpload));
        }
    }

    internal class AppTypeDtoToAppTypeProfile : Profile
    {
        public AppTypeDtoToAppTypeProfile()
        {
            CreateMap<AppTypeDto, AppType>()
                .ForMember(appType => appType.Id, dto => dto.MapFrom(w => w.Id))
                .ForMember(appType => appType.Name, dto => dto.MapFrom(w => w.Name))
                .ForMember(appType => appType.IsSystem, dto => dto.MapFrom(w => w.IsSystem));
        }
    }

    internal class AppDtoToAppProfile : Profile
    {
        public AppDtoToAppProfile()
        {
            CreateMap<AppDto, App>()
                .ForMember(app => app.Id, dto => dto.MapFrom(w => w.Id))
                .ForMember(app => app.Name, dto => dto.MapFrom(w => w.Name))
                .ForMember(app => app.IconUrl, dto => dto.MapFrom(w => w.IconUrl))
                .ForMember(app => app.AppUrl, dto => dto.MapFrom(w => w.AppUrl))
                .ForMember(app => app.Remark, dto => dto.MapFrom(w => w.Remark))
                .ForMember(app => app.Width, dto => dto.MapFrom(w => w.Width))
                .ForMember(app => app.Height, dto => dto.MapFrom(w => w.Height))
                .ForMember(app => app.IsOpenMax, dto => dto.MapFrom(w => w.IsOpenMax))
                .ForMember(app => app.IsFlash, dto => dto.MapFrom(w => w.IsFlash))
                .ForMember(app => app.IsResize, dto => dto.MapFrom(w => w.IsResize))
                .ForMember(app => app.UserId, dto => dto.MapFrom(w => w.UserId))
                .ForMember(app => app.AppAuditState, dto => dto.MapFrom(w => w.AppAuditState))
                .ForMember(app => app.AppReleaseState, dto => dto.MapFrom(w => w.AppReleaseState))
                .ForMember(app => app.AppTypeId, dto => dto.MapFrom(w => w.AppTypeId))
                .ForMember(app => app.AppStyle, dto => dto.MapFrom(w => w.AppStyle))
                .ForMember(app => app.IsIconByUpload, dto => dto.MapFrom(w => w.IsIconByUpload));
        }
    }

    internal class RoleDtoToRoleProfile : Profile
    {
        public RoleDtoToRoleProfile()
        {
            CreateMap<RoleDto, Role>()
               .ForMember(role => role.Name, dto => dto.MapFrom(r => r.Name))
               .ForMember(role => role.RoleIdentity, dto => dto.MapFrom(r => r.RoleIdentity))
               .ForMember(role => role.Remark, dto => dto.MapFrom(r => r.Remark))
               .ForMember(role => role.Id, dto => dto.MapFrom(r => r.Id))
               .ForMember(role => role.Powers, dto => dto.MapFrom(r => r.Powers));
        }
    }

    internal class RoleDtoToUserRoleProfile : Profile
    {
        public RoleDtoToUserRoleProfile()
        {
            CreateMap<RoleDto, UserRole>()
                .ForMember(userRole => userRole.RoleId, dto => dto.MapFrom(d => d.Id));
        }
    }

    internal class LogDtoToLogProfile : Profile
    {
        public LogDtoToLogProfile()
        {
            CreateMap<LogDto, Log>()
                .ForMember(log => log.Id, dto => dto.MapFrom(d => d.Id))
                .ForMember(log => log.LogLevelEnum, dto => dto.MapFrom(d => d.LogLevelEnum))
                .ForMember(log => log.Controller, dto => dto.MapFrom(d => d.Controller))
                .ForMember(log => log.Action, dto => dto.MapFrom(d => d.Action))
                .ForMember(log => log.ExceptionMessage, dto => dto.MapFrom(d => d.ExceptionMessage))
                .ForMember(log => log.Track, dto => dto.MapFrom(d => d.Track))
                .ForMember(log => log.UserId, dto => dto.MapFrom(d => d.UserId));
        }
    }

    internal class VisitorRecordDtoToVisitorRecord : Profile
    {
        public VisitorRecordDtoToVisitorRecord()
        {
            CreateMap<VisitorRecordDto, VisitorRecord>()
                .ForMember(log => log.Id, dto => dto.MapFrom(d => d.Id))
                .ForMember(log => log.UserId, dto => dto.MapFrom(d => d.UserId))
                .ForMember(log => log.UserName, dto => dto.MapFrom(d => d.UserName))
                .ForMember(log => log.Controller, dto => dto.MapFrom(d => d.Controller))
                .ForMember(log => log.Action, dto => dto.MapFrom(d => d.Action))
                .ForMember(log => log.Ip, dto => dto.MapFrom(d => d.Ip))
                .ForMember(log => log.VisitorUrl, dto => dto.MapFrom(d => d.VisitorUrl))
                .ForMember(log => log.UrlParameter, dto => dto.MapFrom(d => d.UrlParameter));
        }
    }
}
