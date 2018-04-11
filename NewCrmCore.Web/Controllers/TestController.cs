using Microsoft.AspNetCore.Mvc;

namespace NewCrmCore.Web.Controllers
{
	public class TestController: Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}
	}
}