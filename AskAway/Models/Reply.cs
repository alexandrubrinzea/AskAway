using AskAway.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskAway.Models
{
    public class Reply
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cotinutul raspunsului este obligatoriu")]
        public string Content { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Data este obligatorie")]
        public DateTime Date { get; set; }

        public bool CorrectAnswer { get; set; }

        public int TopicId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

}