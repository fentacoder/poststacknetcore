using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostStack.Models
{
    public class UserValidation
    {
        [Key]
        public int Id { get; set; } = -1;

        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Your email is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Your email must be greater than 3 characters and less than 150 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Your password is required"),StringLength(50, MinimumLength = 6, ErrorMessage = "Your password must be greater than 6 characters and less than 50 characters"),
            DataType(DataType.Password,ErrorMessage = "Invalid password format")]
        public string Password { get; set; } = "";

        public DateTime UserCreatedAt { get; set; }

        public int IsAdmin { get; set; } = 0;

        public UserValidation()
        {

        }
    }
}
