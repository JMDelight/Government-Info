using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GovernmentInformation.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernmentInformation.Controllers
{
    public class UserController : Controller
    {
        // GET: /<controller>/
        private GovernmentInformationDbContext db = new GovernmentInformationDbContext();
        public IActionResult Index()
        {
            return View(db.Users.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var thisUser = db.Users.FirstOrDefault(user => user.UserId == id);
            var userQueries = db.Queries.Where(location => location.UserId == id);
            Dictionary<string, object> model = new Dictionary<string, object> { };
            model.Add("user", thisUser);
            model.Add("queries", userQueries);
            return View(model);

        }

        public IActionResult Edit(int id)
        {
            var thisUser = db.Users.FirstOrDefault(user => user.UserId == id);
            return View(thisUser);
        }
        [HttpPost]
        public IActionResult Edit(User user)
        {
            db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var thisUser = db.Users.FirstOrDefault(user => user.UserId == id);
            return View(thisUser);
        }
        [HttpPost]
        public IActionResult Delete(User deletedUser)
        {
            var thisUser = db.Users.FirstOrDefault(user => user.UserId == deletedUser.UserId);
            db.Users.Remove(thisUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
