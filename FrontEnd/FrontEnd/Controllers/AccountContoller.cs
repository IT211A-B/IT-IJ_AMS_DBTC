using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ModelState.Clear();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ShowErrors = true;
                ViewBag.ErrorMessage = "Please enter both username and password.";
                return View();
            }

            if (Authenticate(username, password))
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Dashboard", "Account");
            }

            ViewBag.ShowErrors = true;
            ViewBag.ErrorMessage = "Invalid username or password. Please try again.";
            return View();
        }

        public IActionResult Dashboard() => View("Dashboard");
        public IActionResult Students() => View("Students");
        public IActionResult Teachers() => View("Teachers");
        public IActionResult Courses() => View("Courses");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout() => RedirectToAction("Login", "Account");

        private static bool Authenticate(string username, string password)
            => username == "admin" && password == "admin123";
    }
}