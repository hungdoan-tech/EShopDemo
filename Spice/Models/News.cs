using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Header")]
        [Required]
        public string Header { get; set; }
        [Display(Name = "Content")]
        [Required]
        public string Content { get; set; }
        [Display(Name = "Alias")]
        [Required]
        public string Alias { get; set; }
        [Display(Name = "Author")]
        [Required]
        public string Author { get; set; }
        [Display(Name = "DatePublished")]
        [Required]
        public DateTime DatePublished   { get; set; }
        [Display(Name = "Image")]
        public string Image { get; set; }

        [Display(Name = "NewsCategory")]
        public int NewsCategoryId { get; set; }

        [ForeignKey("NewsCategoryId")]
        public virtual NewsCategory NewsCategory { get; set; }
    }
}
