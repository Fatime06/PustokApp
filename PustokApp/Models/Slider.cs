using Microsoft.AspNetCore.Mvc;
using PustokApp.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokApp.Models
{
    public class Slider : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        [MaxLength(20)]
        public string ButtonText { get; set; }
        [Required]
        [MaxLength(100)]
        public string ButtonLink { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        [NotMapped]
        [MaxSize(2*1024*1024)]
        [AllowedTypes("image/jpeg", "image/png")]

        public IFormFile Photo { get; set; }
    }
}
