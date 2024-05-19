using Microsoft.AspNetCore.Mvc;

namespace E_Diary.WEB.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class MainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
