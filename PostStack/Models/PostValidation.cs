using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostStack.Models
{
    public class PostValidation
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter a title for your post")]
        [StringLength(40, MinimumLength = 3,ErrorMessage = "Your title must be greater than 3 characters and less than 40 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter a body for your post")]
        [StringLength(400, MinimumLength = 3, ErrorMessage = "Your body must be greater than 3 characters and less than 400 characters")]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int IdxInList { get; set; }

        public int PageCount { get; set; }

        public PostValidation()
        {

        }
    }
}
