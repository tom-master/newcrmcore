using System;
using System.Collections.Generic;
using System.Linq;
using NewCrmCore.Domain.ValueObject;
using NewLibCore.Data.SQL.Mapper.Extension;
using NewLibCore.Data.SQL.Mapper.Extension.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.Agent
{
    [TableName("newcrm_user")]
    public partial class User : EntityBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required, InputRange(4, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        [Required]
        public String LoginPassword { get; private set; }

        /// <summary>
        /// 锁屏密码
        /// </summary>
        [Required]
        public String LockScreenPassword { get; private set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsDisable { get; private set; }

        /// <summary>
        /// 最后一次登录的时间
        /// </summary>
        [DateTimeDefaultValue]
        public DateTime LastLoginTime { get; private set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsOnline { get; private set; }

        /// <summary>
        /// 是否为管理员
        /// </summary>
        [DefaultValue(typeof(Boolean))]
        public Boolean IsAdmin { get; private set; }

        /// <summary>
        /// 配置Id
        /// </summary>
        [Required]
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
                throw new ArgumentException($@"{nameof(LoginPassword)}不能为空");
            }

            if (password == LoginPassword)
            {
                return this;
            }

            LoginPassword = password;
            OnChanged(nameof(LoginPassword));
            return this;
        }

        /// <summary>
        /// 修改锁屏密码
        /// </summary>
        public User ModifyLockScreenPassword(String password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException($@"{nameof(LockScreenPassword)}不能为空");
            }

            if (password == LockScreenPassword)
            {
                return this;
            }

            LockScreenPassword = password;
            OnChanged(nameof(LockScreenPassword));
            return this;
        }

        /// <summary>
        /// 修改关联配置文件Id
        /// </summary>
        public User ModifyConfigId(Int32 configId)
        {
            ConfigId = configId;
            OnChanged(nameof(ConfigId));
            return this;
        }

        /// <summary>
        /// 账户启用
        /// </summary>
        public User Enable()
        {
            IsDisable = false;
            OnChanged(nameof(IsDisable));
            return this;
        }

        /// <summary>
        /// 账户禁用
        /// </summary>
        public User Disable()
        {
            IsDisable = true;
            OnChanged(nameof(IsDisable));
            return this;
        }

        /// <summary>
        /// 上线
        /// </summary>
        /// <returns></returns>
        public User Online()
        {
            IsOnline = true;
            OnChanged(nameof(IsOnline));
            LastLoginTime = DateTime.Now;
            OnChanged(nameof(LastLoginTime));
            return this;
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <returns></returns>
        public User Offline()
        {
            IsOnline = false;
            OnChanged(nameof(IsOnline));
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
            OnChanged(nameof(Roles));
            return this;
        }

        /// <summary>
        /// 去除管理员角色
        /// </summary>
        public User DetachAdminRole()
        {
            IsAdmin = false;
            OnChanged(nameof(IsAdmin));
            return this;
        }

        /// <summary>
        /// 附加管理员角色
        /// </summary>
        /// <returns></returns>
        public User AttachAdminRole()
        {
            IsAdmin = true;
            OnChanged(nameof(IsAdmin));
            return this;
        }
    }
}
