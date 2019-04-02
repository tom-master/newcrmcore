using System;
using NewLibCore.Data.SQL.Mapper.Extension;
using NewLibCore.Data.SQL.Mapper.Extension.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.System
{
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
            OnPropertyChanged(nameof(Name));
            return this;
        }

        public AppType ModifyRemark(String remark)
        {
            if (remark == Remark)
            {
                return this;
            }

            Remark = remark ?? "";
            OnPropertyChanged(nameof(Remark));
            return this;
        }

        public AppType System()
        {
            IsSystem = true;
            OnPropertyChanged(nameof(IsSystem));
            return this;
        }

        public AppType NotSystem()
        {
            IsSystem = false;
            OnPropertyChanged(nameof(IsSystem));
            return this;
        }
    }
}
