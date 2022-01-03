using AskAway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskAway.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Category
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Index()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }

        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            //ViewBag.Category = category;

            return View(category);
        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New()
        {
            Category category = new Category();

            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New(Category cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();

                    TempData["message"] = "Categoria a fost adaugata cu succes!";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["message"] = "A aparut o eroare la adaugarea categoriei!";

            }
            return View(cat);

        }

        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            //ViewBag.Category = category;

            return View(category);
        }

        [HttpPut]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.CategoryName = requestCategory.CategoryName;
                        db.SaveChanges();
                    }
                    TempData["message"] = "Categoria a fost modificata!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["message"] = "A aparut o eroare la salvarea modificarii!";
            }

            return View(requestCategory);
        }

        [HttpDelete]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["message"] = "Categoria a fost stearsa!";

            return RedirectToAction("Index");
        }
    }
}