using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web.Mvc;
using AskAway.Models;
using Microsoft.AspNet.Identity;

namespace AskAway.Controllers
{
    public class ReplyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Reply
        public ActionResult Index()
        {
            var replies = db.Replies.Include("Category").Include("User");
            ViewBag.reply = replies;

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
            Reply reply = db.Replies.Find(id);

            return View(reply);
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New()
        {
            Reply reply = new Reply();
            reply.UserId = User.Identity.GetUserId();

            return View(reply);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult New(Reply reply, int id)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    reply.TopicId = id;
                    db.Replies.Add(reply);
                    db.SaveChanges();

                    TempData["succesMessage"] = "Raspunsul a fost adaugat cu succes!";

                    return RedirectToAction("Show","Topic", new { id = reply.TopicId });
                }
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "A aparut o eroare la adaugarea raspunsului!";

            }
            return View(reply);

        }

        [Authorize(Roles = "Moderator,Administrator")]
        public ActionResult MarkAnswer(int id)
        {
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

            Reply reply = db.Replies.Find(id);
            
            if (reply == null)
            {
                TempData["errorMessage"] = "A aparut o eroare!";
                return RedirectToAction("Index");
            }

            var otherCorrectAnswers = db.Replies.Where(r => r.TopicId == reply.TopicId && r.CorrectAnswer );

            if (otherCorrectAnswers.Count() > 0)
            {
                TempData["warningMessage"] = "Exista deja un raspuns marcat drept corect.";
            }
            else
            {
                reply.CorrectAnswer = true;
                db.SaveChanges();
            }
            return RedirectToAction("Show", "Topic", new { id = reply.TopicId });
        }

        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id)
        {

            Reply reply = db.Replies.Find(id);
            // topic.Categories = GetAllCategories();

            if (reply.UserId != User.Identity.GetUserId() && (!User.IsInRole("Administrator") || !User.IsInRole("Moderator")))
            {
                TempData["errorMessage"] = "Nu aveti dreptul sa faceti modificari asupra unui raspuns care nu va apartine!";
                return RedirectToAction("Show", "Topic", new { id = reply.TopicId });
            }

            return View(reply);
        }

        [HttpPut]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Edit(int id, Reply requestReply)
        {
            //requestTopic.Categories = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Reply reply = db.Replies.Find(id);

                    if (reply.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator") || User.IsInRole("Moderator"))
                    {
                        if (TryUpdateModel(reply))
                        {
                            reply.Content = requestReply.Content;


                            db.SaveChanges();
                            TempData["succesMessage"] = "Raspunsul a fost modificat!";
                            return RedirectToAction("Show","Topic", new { id = reply.TopicId });
                        }

                    }
                    else
                    {
                        TempData["errorMessage"] = "Nu aveti dreptul sa faceti modificari asupra unui raspuns care nu va apartine!";
                        return RedirectToAction("Show", "Topic", new { id = reply.TopicId });
                    }

                }
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = "A aparut o eroare la salvarea modificarii!";
            }

            return View(requestReply);
        }

        [HttpDelete]
        [Authorize(Roles = "User,Moderator,Administrator")]
        public ActionResult Delete(int id)
        {
            Reply reply = db.Replies.Find(id);
            db.Replies.Remove(reply);
            db.SaveChanges();

            TempData["succesMessage"] = "Raspunsul a fost sters!";

            return RedirectToAction("Show", "Topic", new { id = reply.TopicId });
        }
    }
}