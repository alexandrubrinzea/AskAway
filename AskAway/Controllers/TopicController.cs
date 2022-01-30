using AskAway.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskAway.Controllers
{
    public class TopicController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Category
        public ActionResult Index()
        {
            var topics = db.Topics.Include("Category").Include("User");
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
            Topic topic = db.Topics.Find(id);

            return View(topic);
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New()
        {
            Topic topic = new Topic();

            topic.Categories = GetAllCategories();
            topic.UserId = User.Identity.GetUserId();
            
            return View(topic);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(Topic topic)
        {
            topic.Categories = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Topics.Add(topic);
                    db.SaveChanges();

                    TempData["succesMessage"] = "Subiectul a fost adaugat cu succes!";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "A aparut o eroare la adaugarea Subiectului!";

            }
            return View(topic);

        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id)
        {
            Topic topic = db.Topics.Find(id);
            topic.Categories = GetAllCategories();

            if ( topic.UserId != User.Identity.GetUserId() && ( !User.IsInRole("Administrator") || !User.IsInRole("Moderator") ) )
            {
                TempData["errorMessage"] = "Nu aveti dreptul sa faceti modificari asupra unui subiect care nu va apartine!";
                return RedirectToAction("Index");
            }

            return View(topic);
        }

        [HttpPut]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id, Topic requestTopic)
        {
            requestTopic.Categories = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Topic topic = db.Topics.Find(id);

                    if ( topic.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator") || User.IsInRole("Moderator") )
                    {
                        if (TryUpdateModel(topic))
                        {
                            topic.Title = requestTopic.Title;
                            topic.CategoryId = requestTopic.CategoryId;

                            db.SaveChanges();
                            TempData["succesMessage"] = "Subiectul a fost modificat!";
                            return RedirectToAction("Index");
                        }
                        
                    }
                    else
                    {
                        TempData["errorMessage"] = "Nu aveti dreptul sa faceti modificari asupra unui subiect care nu va apartine!";
                        return RedirectToAction("Index");
                    }
                    
                }
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "A aparut o eroare la salvarea modificarii!";
            }

            return View(requestTopic);
        }

        [HttpDelete]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Delete(int id)
        {
            Topic topic = db.Topics.Find(id);
            db.Topics.Remove(topic);
            db.SaveChanges();

            TempData["succesMessage"] = "Subiectul a fost sters!";

            return RedirectToAction("Index");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();
            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            return selectList;
        }
    }
}