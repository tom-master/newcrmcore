using System;

namespace NewCrmCore.Infrastructure.CommonTools
{
	public abstract class CacheKeyBase
	{
		private Int32 _identity = 0;

		protected CacheKeyBase(Int32 identity)
		{
			_identity = identity;
		}

		protected virtual String CachePrefix
		{
			get
			{
				return "NewCrm";
			}
		}

		public abstract String Key { get; protected set; }

		/// <summary>
		/// 查询超时时间
		/// </summary>
		public virtual TimeSpan CancelToken
		{
			get
			{
				return new TimeSpan(0, 0, 3);
			}
		}

		/// <summary>
		/// 缓存过期时间
		/// </summary>
		public virtual TimeSpan KeyTimeout
		{
			get
			{
				return new TimeSpan(0, 30, 0);
			}
		}

		protected virtual String ProcessKey()
		{
			return String.Format(Key, CachePrefix, _identity);
		}

		public String GetKey()
		{
			return ProcessKey();
		}
	}
}
