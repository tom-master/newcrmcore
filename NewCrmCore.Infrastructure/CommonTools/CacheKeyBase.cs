using System;
using System.Threading;

namespace NewCrmCore.Infrastructure.CommonTools
{
	public abstract class CacheKeyBase
	{
		private CancellationTokenSource _cancellationTokenSource;
		private Int32 randomSeed = new Random(DateTime.Now.Millisecond).Next(1, 20);

		protected CacheKeyBase()
		{
			_cancellationTokenSource = new CancellationTokenSource(new TimeSpan(0, 0, 10));
		}

		/// <summary>
		/// 获取取消标识
		/// </summary>
		public virtual CancellationToken CancelToken
		{
			get
			{
				return _cancellationTokenSource.Token;
			}
		}

		/// <summary>
		/// 缓存过期时间
		/// </summary>
		public virtual TimeSpan KeyTimeout
		{
			get
			{
				return new TimeSpan(0, randomSeed, 0);
			}
		}

		protected abstract String FormatKey();

		public String GetKey()
		{
			return String.Format("NewCrm:{0}", FormatKey());
		}
	}
}
