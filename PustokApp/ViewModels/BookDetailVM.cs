using PustokApp.Models;

namespace PustokApp.ViewModels
{
    public class BookDetailVM
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; }
        public bool HasCommentUser { get; set; }
        public int TotalComments { get; set; }
        public int AvgRate { get; set; }
        public BookComment BookComment { get; set; }
    }
}
