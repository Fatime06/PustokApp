using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PustokApp.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class DashboardController : Controller
    {
        public IActionResult CreateAdmin()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
