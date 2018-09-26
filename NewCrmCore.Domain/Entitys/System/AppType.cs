using System;
using System.ComponentModel;
using NewLibCore.Data.Mapper.MapperExtension;
using NewLibCore.Data.Mapper.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.System
{
    [Description("应用类型"), Serializable]
    public partial class AppType : DomainModelBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        [PropertyRequired, PropertyInputRange(2, 10)]
        public String Name { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [PropertyDefaultValue(typeof(String)), PropertyInputRange(50)]
        public String Remark { get; private set; }

        /// <summary>
        /// 是否为系统分类
        /// </summary>
        [PropertyRequired]
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

            Name = appTypeName;
            OnPropertyChanged(new PropertyArgs(nameof(Name), Name));
            return this;
        }

        public AppType ModifyRemark(String remark)
        {
            if (String.IsNullOrEmpty(remark))
            {
                throw new ArgumentException($@"{nameof(remark)} 不能为空");
            }

            Remark = remark;
            OnPropertyChanged(new PropertyArgs(nameof(Remark), Remark));
            return this;
        }

        public AppType System()
        {
            IsSystem = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsSystem), IsSystem));
            return this;
        }

         public AppType NotSystem()
        {
            IsSystem = false;
            OnPropertyChanged(new PropertyArgs(nameof(IsSystem), IsSystem));
            return this;
        }
    }
}
