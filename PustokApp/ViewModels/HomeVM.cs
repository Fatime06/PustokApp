using PustokApp.Models;

namespace PustokApp.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Book> DiscountBooks { get; set; }
        public List<Author> Author { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
