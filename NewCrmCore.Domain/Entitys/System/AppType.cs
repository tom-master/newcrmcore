using System;
using System.ComponentModel;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCRM.Domain.Entitys.System
{
	[Description("应用类型"), Serializable]
	public partial class AppType : DomainModelBase
	{
		#region private field

		private String _name;

		private String _remark;
		#endregion

		#region public proptery

		/// <summary>
		/// 名称
		/// </summary>
		[PropertyRequired, InputRange(1, 6)]
		public String Name
		{
			get { return _name; }
			private set
			{
				_name = value;
			}
		}

		/// <summary>
		/// 备注
		/// </summary>
		[PropertyDefaultValue(typeof(String), ""), InputRange(50)]
		public String Remark
		{
			get { return _remark; }
			private set
			{
				_remark = value;
			}
		}

		#endregion

		#region ctor
		/// <summary>
		/// 实例化一个app类型对象
		/// </summary>
		public AppType(String name, String remark = default(String))
		{
			Name = name;
			Remark = remark;
		}

		public AppType()
		{
		}

		#endregion
	}

	/// <summary>
	/// AppTypeExtension
	/// </summary>
	public partial class AppType
	{
		public AppType ModifyName(String appTypeName)
		{
			if(String.IsNullOrEmpty(appTypeName))
			{
				throw new ArgumentException($@"{nameof(appTypeName)} is null");
			}

			Name = appTypeName;
			OnPropertyChanged(nameof(Name));
			return this;
		}

		public AppType ModifyRemark(String remark)
		{
			if(String.IsNullOrEmpty(remark))
			{
				throw new ArgumentException($@"{nameof(remark)} is null");
			}

			Remark = remark;
			OnPropertyChanged(nameof(Remark));
			return this;
		}
	}
}
