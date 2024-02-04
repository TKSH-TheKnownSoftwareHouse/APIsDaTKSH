using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIsDaTKSH.Models
{
    public class DepositionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required(ErrorMessage = "The author of the deposition is required.")]
        [Column("author")]  
        public string Author { get; set; }

        [Required(ErrorMessage = "The main message is required.")]
        [Column("main_message")]  
        public string MainMessage { get; set; }

        [Required(ErrorMessage = "The subtitle is required.")]
        [Column("subtitle")]  
        public string Subtitle { get; set; }

        [Required(ErrorMessage = "The rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [Column("rating")]  
        public int Rating { get; set; }
    }
}
