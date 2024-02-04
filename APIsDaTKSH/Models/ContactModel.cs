using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIsDaTKSH.Models
{
    public class ContactModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int id { get; set; }

        [Required(ErrorMessage = "The full name is required.")]
        [StringLength(100, ErrorMessage = "The full name cannot exceed 100 characters.")]
        [Column("full_name")]  
        public string FullName { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Column("email")]  
        public string Email { get; set; }

        [Required(ErrorMessage = "The message is required.")]
        [Column("message")] 
        public string Message { get; set; }
    }
}
