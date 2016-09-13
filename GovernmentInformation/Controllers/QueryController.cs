using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GovernmentInformation.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernmentInformation.Controllers
{
    public class QueryController : Controller
    {
        private GovernmentInformationDbContext db = new GovernmentInformationDbContext();
        public IActionResult Index()
        {
            return View(db.Queries.Include(queries => queries.User).ToList());
            
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Query query)
        {
            db.Queries.Add(query);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var thisQuery = db.Queries.FirstOrDefault(query => query.QueryId == id);
            var queriesUser = db.Users.Where(user => user.UserId == thisQuery.UserId);
            Dictionary<string, object> model = new Dictionary<string, object> { };
            model.Add("query", thisQuery);
            model.Add("user", queriesUser);
            return View(model);

        }

        public IActionResult Edit(int id)
        {
            var thisQuery = db.Queries.FirstOrDefault(query => query.QueryId == id);
            return View(thisQuery);
        }
        [HttpPost]
        public IActionResult Edit(Query query)
        {
            db.Entry(query).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var thisQuery = db.Queries.FirstOrDefault(query => query.QueryId == id);
            return View(thisQuery);
        }
        [HttpPost]
        public IActionResult Delete(Query deletedQuery)
        {
            var thisQuery = db.Queries.FirstOrDefault(query => query.QueryId == deletedQuery.QueryId);
            db.Queries.Remove(thisQuery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
