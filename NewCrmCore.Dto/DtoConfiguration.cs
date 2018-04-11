using System.Collections.Generic;
using AutoMapper;
using NewCrmCore.Domain.Entitys;
using NewCrmCore.Domain.Entitys.Agent;
using NewCrmCore.Domain.Entitys.Security;
using NewCrmCore.Domain.Entitys.System;
using NewCrmCore.Dto.MapperProfile;

namespace NewCrmCore.Dto
{
	public static class DtoConfiguration
	{
		static DtoConfiguration()
		{

			Mapper.Initialize(d =>
			{
				//Account
				//d.CreateMap<Account, AccountDto>();
				//d.AddProfile<AccountToAccountDtoProfile>();

				d.CreateMap<AccountDto, Account>();
				d.AddProfile<AccountDtoToAccountProfile>();

				d.CreateMap<RoleDto, AccountRole>();
				d.AddProfile<RoleDtoToAccountRoleProfile>();

				//d.CreateMap<AccountRole, RoleDto>();
				//d.AddProfile<AccountRoleToRoleDtoProfile>();

				////Wallpaper
				//d.CreateMap<Config, ConfigDto>();
				//d.AddProfile<ConfigToConfigDtoProfile>();

				////Wallpaper
				//d.CreateMap<Wallpaper, WallpaperDto>();
				//d.AddProfile<WallpaperToWallpaperDtoProfile>();

				d.CreateMap<WallpaperDto, Wallpaper>();
				d.AddProfile<WallpaperDtoToWallpaperProfile>();

				//Member
				//d.CreateMap<Member, MemberDto>();
				//d.AddProfile<MemberToMemberDtoProfile>();

				d.CreateMap<MemberDto, Member>();
				d.AddProfile<MemberDtoToMemberProfile>();

				//AppType
				//d.CreateMap<AppType, AppTypeDto>();
				//d.AddProfile<AppTypeToAppTypeDtoProfile>();

				d.CreateMap<AppTypeDto, AppType>();
				d.AddProfile<AppTypeDtoToAppTypeProfile>();

				//App
				//d.CreateMap<App, AppDto>();
				//d.AddProfile<AppToAppDtoProfile>();

				d.CreateMap<AppDto, App>();
				d.AddProfile<AppDtoToAppProfile>();

				//Role
				//d.CreateMap<Role, RoleDto>();
				//d.AddProfile<RoleToRoleDtoProfile>();

				d.CreateMap<RoleDto, Role>();
				d.AddProfile<RoleDtoToRoleProfile>();

				//Desk
				//d.CreateMap<Desk, DeskDto>();
				//d.AddProfile<DeskToDeskDtoProfile>();

				//d.CreateMap<DeskDto, Desk>();
				//d.AddProfile<DeskDtoToDeskProfile>();

				//Log
				//d.CreateMap<Log, LogDto>();
				//d.AddProfile<LogToLogDtoProfile>();

				d.CreateMap<LogDto, Log>();
				d.AddProfile<LogDtoToLogProfile>();

			});

			#region Member

			#region dto ->domain
			//Mapper.CreateMap<MemberDto, Member>()
			//.ForMember(member => member.Id, dto => dto.MapFrom(w => w.Id))
			//.ForMember(member => member.AppId, dto => dto.MapFrom(w => w.AppId))
			//.ForMember(member => member.Width, dto => dto.MapFrom(w => w.Width))
			//.ForMember(member => member.Height, dto => dto.MapFrom(w => w.Height))
			//.ForMember(member => member.FolderId, dto => dto.MapFrom(w => w.FolderId))
			//.ForMember(member => member.Name, dto => dto.MapFrom(w => w.Name))
			//.ForMember(member => member.IconUrl, dto => dto.MapFrom(w => w.IconUrl))
			//.ForMember(member => member.MemberType, dto => dto.MapFrom(w => w.MemberType.ToString().ToLower()))

			//.ForMember(member => member.IsOnDock, dto => dto.MapFrom(w => w.IsOnDock))
			//.ForMember(member => member.IsMax, dto => dto.MapFrom(w => w.IsMax))
			//.ForMember(member => member.IsFull, dto => dto.MapFrom(w => w.IsFull))
			//.ForMember(member => member.IsSetbar, dto => dto.MapFrom(w => w.IsSetbar))
			//.ForMember(member => member.IsOpenMax, dto => dto.MapFrom(w => w.IsOpenMax))
			//.ForMember(member => member.IsLock, dto => dto.MapFrom(w => w.IsLock))
			//.ForMember(member => member.IsFlash, dto => dto.MapFrom(w => w.IsFlash))
			//.ForMember(member => member.IsDraw, dto => dto.MapFrom(w => w.IsDraw))
			//.ForMember(member => member.IsResize, dto => dto.MapFrom(w => w.IsResize))

			//.ForMember(member => member.AppUrl, dto => dto.MapFrom(w => w.AppUrl));
			#endregion


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
			where TModel : DomainModelBase => Mapper.Map<TModel>(source);

		public static IEnumerable<TModel> ConvertToModels<TDto, TModel>(this IEnumerable<TDto> source)
			where TDto : BaseDto
			where TModel : DomainModelBase => Mapper.Map<IEnumerable<TModel>>(source);


		public static TModel ConvertToModel<TDto, TModel>(this TDto source, TModel tt)
			 where TDto : BaseDto
			 where TModel : DomainModelBase => Mapper.Map(source, tt);

		#endregion
	}
}
