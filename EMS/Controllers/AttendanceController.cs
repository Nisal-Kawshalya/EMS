using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
