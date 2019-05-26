using System.Web.Mvc;

namespace LgWG.LogQuery.Web.Controllers
{
    public class AboutController : LogQueryControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}