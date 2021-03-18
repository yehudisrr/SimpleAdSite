using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsAuth.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Web.Controllers
{
    public class Account : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Ads;Integrated Security=True";

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var db = new AdsDb(_connectionString);
            db.AddUser(user, password);
            return Redirect("/account/login");
        }

        public IActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var db = new AdsDb(_connectionString);
            var user = db.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid email/password - please try again.";
                return Redirect("/account/login");
            }

            var claims = new List<Claim>
            {
                new Claim("user", email) 
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/newad");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}
