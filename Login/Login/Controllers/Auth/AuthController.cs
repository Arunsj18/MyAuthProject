using Microsoft.AspNetCore.Mvc;

namespace Login.Controllers.Auth
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}