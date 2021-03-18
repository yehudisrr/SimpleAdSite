using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Ads;Integrated Security=True";

        public IActionResult Index()
        {
            var db = new AdsDb(_connectionString);
            var vm = new AdsViewModel();
            vm.Ads = db.GetAds();
            vm.IsAuthenticated = User.Identity.IsAuthenticated;

            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                vm.CurrentUser = db.GetByEmail(email);
            }

            return View(vm);

        }
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            AdsDb db = new AdsDb(_connectionString);
            ad.Date = DateTime.Now;
            User CurrentUser = db.GetByEmail(User.Identity.Name);
            ad.UserId = CurrentUser.Id;
            db.AddAd(ad);
                      
            return Redirect("/Home/index");
        }

        public IActionResult MyAccount()
        {
            var db = new AdsDb(_connectionString);
            var vm = new AdsViewModel();
            User CurrentUser = db.GetByEmail(User.Identity.Name);
            vm.Ads = db.GetAds().Where(a => a.UserId == CurrentUser.Id).ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            AdsDb db = new AdsDb(_connectionString);
            db.DeleteAd(id);
            return Redirect("/Home/Index");
        }

    }
}
