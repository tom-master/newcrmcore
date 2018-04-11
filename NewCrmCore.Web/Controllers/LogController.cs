using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NewCrmCore.Web.Controllers.ControllerHelper;

namespace NewCrmCore.Web.Controllers
{
	public class LogController : BaseController
	{
		private readonly ILoggerServices _loggerServices;

		public LogController(ILoggerServices loggerServices)
		{
			_loggerServices = loggerServices;
		}

		#region 页面
		/// <summary>
		/// 首页
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		#endregion

		/// <summary>
		/// 获取日志列表
		/// </summary>
		[HttpGet]
		public ActionResult GetLogs(Int32 accountId, Int32 loglevel, Int32 pageIndex, Int32 pageSize)
		{
			var response = new ResponseModels<IList<LogDto>>();
			var result = _loggerServices.GetLogs(accountId, loglevel, pageIndex, pageSize, out var totalCount);
			response.IsSuccess = true;
			response.Message = "获取日志列表成功";
			response.Model = result;
			response.TotalCount = totalCount;

			return Json(response, JsonRequestBehavior.AllowGet);
		}
	}
}