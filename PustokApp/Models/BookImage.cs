namespace PustokApp.Models
{
    public class BookImage : BaseEntity
    {
        public string Name { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public bool? Status { get; set; } // true false null
    }
}
