using System;
using NewLibCore.Storage.SQL;
using NewLibCore.Storage.SQL.Validate;

namespace NewCrmCore.Domain.Entitys.System
{
    [TableName("newcrm_app_type")]
    public partial class AppType : EntityBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required, InputRange(2, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DefaultValue(typeof(String)), InputRange(50)]
        public String Remark { get; private set; }

        /// <summary>
        /// 是否为系统分类
        /// </summary>
        [Required]
        public Boolean IsSystem { get; private set; }

        /// <summary>
        /// 实例化一个app类型对象
        /// </summary>
        public AppType(String name, Boolean isSystem, String remark = "")
        {
            Name = name;
            IsSystem = isSystem;
            Remark = remark;
        }



        public AppType()
        {
        }
    }

    public partial class AppType
    {
        public AppType ModifyName(String appTypeName)
        {
            if (String.IsNullOrEmpty(appTypeName))
            {
                throw new ArgumentException($@"{nameof(appTypeName)} 不能为空");
            }

            if (appTypeName == Name)
            {
                return this;
            }

            Name = appTypeName;
            OnChanged(nameof(Name));
            return this;
        }

        public AppType ModifyRemark(String remark)
        {
            if (remark == Remark)
            {
                return this;
            }

            Remark = remark ?? "";
            OnChanged(nameof(Remark));
            return this;
        }

        public AppType System()
        {
            IsSystem = true;
            OnChanged(nameof(IsSystem));
            return this;
        }

        public AppType NotSystem()
        {
            IsSystem = false;
            OnChanged(nameof(IsSystem));
            return this;
        }
    }
}
