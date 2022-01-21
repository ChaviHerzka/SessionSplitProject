using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SplitProject.Data;
using SplitProject.Web.Models;
using System.Diagnostics;

namespace SplitProject.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Adds;Integrated Security=true;";
        public IActionResult Index()
        {
           
            ListingDB db = new(_connectionString);
            ListingViewModel vm = new ListingViewModel();

            List<int> ids = HttpContext.Session.Get<List<int>>("Ids");
            if (ids != null)
            {
                vm.Ids = ids;
            }

            vm.Listings = db.GetAds();

           
            return View(vm);
        }
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(Listing list)
        {  
            ListingDB db = new(_connectionString);
            db.AddList(list);
            List<int> ids = HttpContext.Session.Get<List<int>>("Ids");
            if (ids == null)
            {
                ids = new List<int>();
            }
            ids.Add(list.Id);
            HttpContext.Session.Set("Ids", ids);
            return Redirect("/Home/Index");
        }


        [HttpPost]
        public IActionResult Delete(int id) 
        {
            ListingDB db = new(_connectionString);
            db.DeleteAds(id);
            return Redirect("/");
        }
    }
}