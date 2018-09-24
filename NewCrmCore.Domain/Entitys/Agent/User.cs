using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.Mapper.MapperExtension;
using NewLibCore.Data.Mapper.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.Agent
{
    [Description("用户"), Serializable]
    public partial class User : DomainModelBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [PropertyRequired, PropertyInputRange(4, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        [PropertyRequired]
        public String LoginPassword { get; private set; }

        /// <summary>
        /// 锁屏密码
        /// </summary>
        [PropertyRequired]
        public String LockScreenPassword { get; private set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsDisable { get; private set; }

        /// <summary>
        /// 最后一次登录的时间
        /// </summary>
        [DateTimeDefaultValue]
        public DateTime LastLoginTime { get; private set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsOnline { get; private set; }

        /// <summary>
        /// 是否为管理员
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsAdmin { get; private set; }

        /// <summary>
        /// 配置Id
        /// </summary>
        [PropertyRequired]
        public Int32 ConfigId { get; private set; }

        /// <summary>
        /// 账户头像
        /// </summary>
        public String UserFace { get; private set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public IEnumerable<UserRole> Roles { get; private set; }

        public Boolean IsModifyUserFace { get; private set; }

        /// <summary>
        /// 实例化一个用户对象
        /// </summary>
        public User(String name, String password, IEnumerable<UserRole> roles, UserType userType = default(UserType))
        {
            Name = name;
            LoginPassword = password;
            IsDisable = false;
            LastLoginTime = DateTime.Now;
            LockScreenPassword = password;
            IsOnline = false;
            IsAdmin = userType == UserType.Admin;
            Roles = roles;
        }

        public User() { }
    }

    public partial class User
    {
        /// <summary>
        /// 修改登陆密码
        /// </summary>
        public User ModifyLoginPassword(String password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException($@"{nameof(LoginPassword)} is null");
            }

            LoginPassword = password;
            OnPropertyChanged(new PropertyArgs(nameof(LoginPassword), password));
            return this;
        }

        /// <summary>
        /// 修改锁屏密码
        /// </summary>
        public User ModifyLockScreenPassword(String password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException($@"{nameof(LockScreenPassword)} is null");
            }

            LockScreenPassword = password;
            OnPropertyChanged(new PropertyArgs(nameof(LockScreenPassword), password));
            return this;
        }

        /// <summary>
        /// 修改关联配置文件Id
        /// </summary>
        public User ModifyConfigId(Int32 configId)
        {
            ConfigId = configId;
            OnPropertyChanged(new PropertyArgs(nameof(ConfigId), configId));
            return this;
        }

        /// <summary>
        /// 账户启用
        /// </summary>
        public User Enable()
        {
            IsDisable = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsDisable), IsDisable));
            return this;
        }

        /// <summary>
        /// 账户禁用
        /// </summary>
        public User Disable()
        {
            IsDisable = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsDisable), IsDisable));
            return this;
        }

        /// <summary>
        /// 上线
        /// </summary>
        /// <returns></returns>
        public User Online()
        {
            IsOnline = true;
            LastLoginTime = DateTime.Now;
            OnPropertyChanged(new PropertyArgs(nameof(IsOnline), IsOnline), new PropertyArgs(nameof(LastLoginTime), LastLoginTime));
            return this;
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <returns></returns>
        public User Offline()
        {
            IsOnline = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsOnline), IsOnline));
            return this;
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        public User ModifyRoles(params Int32[] roleIds)
        {
            if (roleIds.Length == 0)
            {
                return this;
            }

            Roles.ToList().Clear();
            Roles = roleIds.Select(roleId => new UserRole(Id, roleId));
            OnPropertyChanged(new PropertyArgs(nameof(Roles), Roles));
            return this;
        }

        /// <summary>
        /// 去除管理员角色
        /// </summary>
        public User DetachAdminRole()
        {
            IsAdmin = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsAdmin), IsAdmin));
            return this;
        }

        /// <summary>
        /// 附加管理员角色
        /// </summary>
        /// <returns></returns>
        public User AttachAdminRole()
        {
            IsAdmin = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsAdmin), IsAdmin));
            return this;
        }
    }
}
