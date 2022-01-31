using AskAway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskAway.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Category
        public ActionResult Index()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            var topics = db.Topics.Include("Category");
            ViewBag.Topics = topics;

            if (TempData.ContainsKey("succesMessage"))
            {
                ViewBag.succesMessage = TempData["succesMessage"].ToString();
            }
            if (TempData.ContainsKey("errorMessage"))
            {
                ViewBag.errorMessage = TempData["errorMessage"].ToString();
            }
            if (TempData.ContainsKey("warningMessage"))
            {
                ViewBag.warningMessage = TempData["warningMessage"].ToString();
            }
            if (TempData.ContainsKey("infoMessage"))
            {
                ViewBag.infoMessage = TempData["infoMessage"].ToString();
            }

            return View();
        }

        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);

            return View(category);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult New()
        {
            Category category = new Category();

            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult New(Category cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();

                    TempData["succesMessage"] = "Categoria a fost adaugata cu succes!";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "A aparut o eroare la adaugarea categoriei!";

            }
            return View(cat);

        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);

            return View(category);
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
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
                    TempData["succesMessage"] = "Categoria a fost modificata!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "A aparut o eroare la salvarea modificarii!";
            }

            return View(requestCategory);
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["succesMessage"] = "Categoria a fost stearsa!";

            return RedirectToAction("Index");
        }
    }
}