using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIsDaTKSH.Models
{
    public class DepositionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indicates that the column is auto-incremented
        public int id { get; set; }

        [Required(ErrorMessage = "The author of the deposition is required.")]
        [Column("author")]  // Maps to the 'author' column in the database
        public string Author { get; set; }

        [Required(ErrorMessage = "The main message is required.")]
        [Column("main_message")]  // Maps to the 'main_message' column in the database
        public string MainMessage { get; set; }

        [Required(ErrorMessage = "The subtitle is required.")]
        [Column("subtitle")]  // Maps to the 'subtitle' column in the database
        public string Subtitle { get; set; }

        [Required(ErrorMessage = "The rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [Column("rating")]  // Maps to the 'rating' column in the database
        public int Rating { get; set; }
    }
}
