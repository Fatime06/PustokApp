using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace PustokApp.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            setItemCookie();
            var result = getItemFromCookie();
            return Content(result);
        } 
        public IActionResult setItemCookie()
        {
            var option = new CookieOptions();
            option.MaxAge = TimeSpan.FromSeconds(5);
            //option.Expires = DateTimeOffset.Now.AddSeconds(5);

            Response.Cookies.Append("group", "pb202");
            return Content("");
        }
        public string getItemFromCookie()
        {
            var group = Request.Cookies["group"];

            return group;
        }
        //public IActionResult setItemToSession()
        //{
        //    HttpContext.Session.SetString("group", "pb202");
        //    return Content("");
        //} 
        //public IActionResult getItemFromSession()
        //{
        //    var group = HttpContext.Session.GetString("group");

        //    return View(group);
        //}
    }
}
