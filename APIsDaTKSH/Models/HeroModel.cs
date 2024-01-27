using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIsDaTKSH.Models
{
    public class HeroModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indicates that the column is auto-incremented
        public int id { get; set; }

        [Required(ErrorMessage = "The main message is required.")]
        [Column("main_message")]  // Maps to the 'main_message' column in the database
        public string MainMessage { get; set; }

        [Required(ErrorMessage = "The subtitle is required.")]
        [Column("subtitle")]  // Maps to the 'subtitle' column in the database
        public string Subtitle { get; set; }


    }
}
