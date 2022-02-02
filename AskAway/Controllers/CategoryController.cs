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
            ViewBag.Topics = GetAllTopics().Skip(0).Take(4);

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

        public ActionResult Show(int id, string sort = "Date", string sortDir = "descending", string search = "%")
        {
            int page = 1;
            int pageSize = 3;
            int totalRecord = 0;
            Int32.TryParse(Request.Params.Get("page"), out page);
            if (page < 1)
                page = 1;
            
            

            int skip = (page * pageSize) - pageSize;

            Category category = db.Categories.Find(id);
            category.Topics = GetTopics(category.CategoryId, search, sort, sortDir, skip, pageSize, out totalRecord);

            ViewBag.TotalRows = totalRecord;
            ViewBag.search = search.Replace("%", "");
            ViewBag.lastPage = Math.Ceiling((float)totalRecord / (float)pageSize);
            ViewBag.page = page;

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

        [NonAction]
        public IEnumerable<Topic> GetTopics(int categoryId, string search, string sort, string sortDir, int skip, int pageSize, out int totalRecord)
        {
            var topics = (from topic in db.Topics
                          //join reply in db.Replies on topic.Id equals reply.TopicId
                          where (
                          topic.CategoryId == categoryId &&
                          (   topic.Title.ToLower().Contains(search.ToLower()) ||
                              topic.Category.CategoryName.ToLower().Contains(search.ToLower()) ||
                              //reply.Content.ToLower().Contains(search.ToLower()) || 
                              search.ToLower().Equals("%")  )
                          )
                          select topic);

            topics = topics.Distinct();
            totalRecord = topics.Count();

            if (!sortDir.Equals("") && !sort.Equals(""))
            {
                if (sortDir.ToLower().Equals("ascending"))
                {
                    if (sort.ToLower().Equals("date"))
                        topics = topics.OrderBy(o => o.Date);
                    else
                        topics = topics.OrderBy(o => o.Title);
                }
                else
                {
                    if (sort.ToLower().Equals("date"))
                        topics = topics.OrderByDescending(o => o.Date);
                    else
                        topics = topics.OrderByDescending(o => o.Title);
                }
            }

            if (pageSize > 0)
            {
                topics = topics.Skip(skip).Take(pageSize);
            }

            var topicList = new List<Topic>();

            foreach (var topic in topics)
            {
                topicList.Add(topic);
            }

            return topicList;
        }

        [NonAction]
        public IEnumerable<Topic> GetAllTopics()
        {
            var topics = from topic in db.Topics
                         select topic;

            var topicList = new List<Topic>();

            foreach (var topic in topics)
            {
                topicList.Add(topic);
            }

            return topicList;
        }

        [NonAction]
        public IEnumerable<Topic> GetSpecificTopics(int CategoryId)
        {
            var topics = from topic in db.Topics
                             select topic;

            var topicList = new List<Topic>();

            foreach (var topic in topics)
            {
                if (topic.CategoryId == CategoryId)
                {
                    topicList.Add(topic);
                }
            }

            return topicList;
        }
    }
}