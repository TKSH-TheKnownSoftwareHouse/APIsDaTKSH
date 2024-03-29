﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIsDaTKSH.Models
{
    public class HeroModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int id { get; set; }

        [Required(ErrorMessage = "The main message is required.")]
        [Column("main_message")] 
        public string MainMessage { get; set; }

        [Required(ErrorMessage = "The subtitle is required.")]
        [Column("subtitle")]  
        public string Subtitle { get; set; }


    }
}
