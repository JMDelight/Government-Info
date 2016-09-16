using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GovernmentInformation.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GovernmentInformation.Controllers
{
    [Authorize]
    public class QueryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public QueryController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            return View(_db.Queries.Where(queries => queries.User.Id == currentUser.Id).ToList());           
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Query query)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            query.User = currentUser;
            _db.Queries.Add(query);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var thisQuery = _db.Queries.Include(u => u.User).FirstOrDefault(query => query.QueryId == id);
            return View(thisQuery);
        }

        public IActionResult Edit(int id)
        {
            var thisQuery = _db.Queries.Include(u => u.User).FirstOrDefault(query => query.QueryId == id);
            return View(thisQuery);
        }
        [HttpPost]
        public IActionResult Edit(Query query)
        {
            _db.Entry(query).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var thisQuery = _db.Queries.FirstOrDefault(query => query.QueryId == id);
            return View(thisQuery);
        }
        [HttpPost]
        public IActionResult Delete(Query deletedQuery)
        {
            var thisQuery = _db.Queries.FirstOrDefault(query => query.QueryId == deletedQuery.QueryId);
            _db.Queries.Remove(thisQuery);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
