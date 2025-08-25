using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySecurityBank.Models;
using System.Diagnostics;

namespace MySecurityBank.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index() => View();
        [Authorize]
        public IActionResult Dashboard() => View();
    

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
