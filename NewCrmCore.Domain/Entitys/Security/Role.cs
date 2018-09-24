using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewLibCore.Data.Mapper.MapperExtension;
using NewLibCore.Data.Mapper.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.Security
{
    [Description("角色"), Serializable]
    public partial class Role : DomainModelBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [PropertyRequired, PropertyInputRange(2, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 角色标识
        /// </summary>
        [PropertyRequired, PropertyInputRange(2, 20)]
        public String RoleIdentity { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [PropertyInputRange(50), PropertyDefaultValue(typeof(String), "")]
        public String Remark { get; private set; }

        /// <summary>
        /// 是否允许禁用
        /// </summary>
        [PropertyDefaultValue(typeof(Boolean), false)]
        public Boolean IsAllowDisable { get; private set; }

        /// <summary>
        /// 权限
        /// </summary>
        public IEnumerable<RolePower> Powers { get; private set; }

        /// <summary>
        /// 实例化一个角色对象
        /// </summary>
        public Role(String name, String roleIdentity, String remark = default(String), Boolean isAllowDisable = default(Boolean))
        {
            Name = name;
            Remark = remark;
            RoleIdentity = roleIdentity;
            IsAllowDisable = isAllowDisable;
            Powers = new List<RolePower>();
        }

        /// <summary>
        /// 实例化一个角色对象
        /// </summary>
        public Role()
        {
        }
    }

    /// <summary>
    /// RoleExtension
    /// </summary>
    public partial class Role
    {
        /// <summary>
        /// 修改角色名称
        /// </summary>
        public Role ModifyRoleName(String roleName)
        {
            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException($@"{nameof(roleName)} is null");
            }

            Name = roleName;
            OnPropertyChanged(new PropertyArgs(nameof(Name), Name));
            return this;
        }

        /// <summary>
        /// 修改角色权限
        /// </summary>
        public Role ModifyRolePower(params Int32[] appIds)
        {
            if (appIds.Length == 0)
            {
                return this;
            }

            Powers.ToList().Clear();
            Powers = appIds.Select(appId => new RolePower(Id, appId));
            OnPropertyChanged(new PropertyArgs(nameof(Powers), Powers));
            return this;
        }
    }
}
