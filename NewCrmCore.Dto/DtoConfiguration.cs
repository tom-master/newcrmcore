using System.Collections.Generic;
using AutoMapper;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Dto.MapperProfile;
using NewLibCore.Storage.SQL;

namespace NewCrmCore.Dto
{
    public static class DtoConfiguration
    {
        static DtoConfiguration()
        {
            Mapper.Initialize(d =>
            {
                d.CreateMap<UserDto, User>();
                d.AddProfile<UserDtoToUserProfile>();

                d.CreateMap<RoleDto, UserRole>();
                d.AddProfile<RoleDtoToUserRoleProfile>();

                d.CreateMap<WallpaperDto, Wallpaper>();
                d.AddProfile<WallpaperDtoToWallpaperProfile>();

                d.CreateMap<MemberDto, Member>();
                d.AddProfile<MemberDtoToMemberProfile>();

                d.CreateMap<AppTypeDto, AppType>();
                d.AddProfile<AppTypeDtoToAppTypeProfile>();

                d.CreateMap<AppDto, App>();
                d.AddProfile<AppDtoToAppProfile>();

                d.CreateMap<RoleDto, Role>();
                d.AddProfile<RoleDtoToRoleProfile>();

                d.CreateMap<LogDto, Log>();
                d.AddProfile<LogDtoToLogProfile>();

                d.CreateMap<VisitorRecordDto, VisitorRecord>();
                d.AddProfile<VisitorRecordDtoToVisitorRecord>();

            });

            #region Member

            #endregion
        }

        #region DtoToDomainModel
        /// <summary>
        /// DTO转换成领域模型
        /// </summary>
        /// <typeparam name="TDto">DTO</typeparam>
        /// <typeparam name="TModel">领域模型</typeparam>
        /// <param name="source">DTO</param>
        /// <returns></returns>
        public static TModel ConvertToModel<TDto, TModel>(this TDto source)
            where TDto : BaseDto
            where TModel : EntityBase => Mapper.Map<TModel>(source);

        public static IEnumerable<TModel> ConvertToModels<TDto, TModel>(this IEnumerable<TDto> source)
            where TDto : BaseDto
            where TModel : EntityBase => Mapper.Map<IEnumerable<TModel>>(source);


        public static TModel ConvertToModel<TDto, TModel>(this TDto source, TModel tt)
             where TDto : BaseDto
             where TModel : EntityBase => Mapper.Map(source, tt);

        #endregion
    }
}
