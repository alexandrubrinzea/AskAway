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
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campul titlu este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere.")]
        public string Title { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Data este obligatorie")]
        public DateTime Date { get; set; }

        public bool ClosedTopic { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual IEnumerable<Reply> Replies { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

}