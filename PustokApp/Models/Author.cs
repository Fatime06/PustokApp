using System.ComponentModel.DataAnnotations;

namespace PustokApp.Models
{
    public class Author : BaseEntity
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
