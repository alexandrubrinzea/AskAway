using AskAway.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskAway.Controllers
{
    [Authorize(Roles = "User,Moderator,Administrator")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        // GET: Users
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;

            return View();
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(string id)
        {
            if (id != User.Identity.GetUserId() && !User.IsInRole("Administrator"))
            {
                TempData["errorMessage"] = "Nu aveti acces la aceasta pagina!";
                return RedirectToAction("Index");
            }

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            return View(user);
        }

        [HttpPut]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(string id, ApplicationUser newData)
        {
            if (id != User.Identity.GetUserId() && !User.IsInRole("Administrator"))
            {
                TempData["errorMessage"] = "Nu aveti dreptul sa modificati aceste date!";
                return RedirectToAction("Index");
            }

            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                if (TryUpdateModel(user))
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;
                    user.PhoneNumber = newData.PhoneNumber;
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }
                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                return View(user);
            }
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            var roles = db.Roles.ToList();

            var roleName = roles.Where(j => j.Id ==
                        user.Roles.FirstOrDefault().RoleId).Select(a => a.Name).FirstOrDefault();
            ViewBag.roleName = roleName;

            return View(user);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        
    }
}