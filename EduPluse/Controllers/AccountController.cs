using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
