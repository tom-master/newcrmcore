using System;
using System.Collections.Generic;
using System.Linq;
using NewLibCore.Data.SQL;
using NewLibCore.Data.SQL.Validate;

namespace NewCrmCore.Domain.Entitys.Security
{

    [TableName("newcrm_role")]
    public partial class Role : EntityBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required, InputRange(2, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 角色标识
        /// </summary>
        [Required, InputRange(2, 20)]
        public String RoleIdentity { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [InputRange(50), DefaultValue(typeof(String))]
        public String Remark { get; private set; }

        /// <summary>
        /// 是否允许禁用
        /// </summary>
        [DefaultValue(typeof(Boolean))]
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
                throw new ArgumentException($@"{nameof(roleName)} 不能为空");
            }

            if (roleName == Name)
            {
                return this;
            }

            Name = roleName;
            OnChanged(nameof(Name));
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
            OnChanged(nameof(Powers));
            return this;
        }
    }
}
