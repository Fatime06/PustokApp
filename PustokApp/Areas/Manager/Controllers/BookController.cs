using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Helpers;
using PustokApp.Models;

namespace PustokApp.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles ="admin, superAdmin")]

    public class BookController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;


        public BookController(PustokDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1, int take = 4)
        {
            var query = _context.Books
                .Include(g => g.Author)
                .Include(g => g.Genre)
                .Include(g => g.BookImages)
                .AsQueryable();
            return View(PaginatedList<Book>.Create(query, take, page));
        }
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }


        public IActionResult Detail(int? id)
        {
            if (id is null)
                return NotFound();
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookTags)
                .ThenInclude(bt=>bt.Tag)
                .Include(b => b.BookImages)
                .FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();
            return View(book);
        }

        public IActionResult DeleteBookImage(int? id)
        {
            if(id is null)
                return NotFound();
            var bookImage = _context.BookImages.Find(id);
            if (bookImage is null)
                return NotFound();
            if (bookImage.Status == true)
            {
                return BadRequest();
            }

            _context.BookImages.Remove(bookImage);
            _context.SaveChanges();
            return RedirectToAction("Detail", new {id=bookImage.BookId});
        }
        
        public IActionResult SetMainImage(int? id)
        {
            if (id is null)
                return NotFound();
            var bookImage = _context.BookImages.Find(id);
            if (bookImage is null)
                return NotFound();

            var mainImage = _context.BookImages.FirstOrDefault(bi => bi.Status == true && bi.BookId == bookImage.BookId);
            mainImage.Status = false;

            bookImage.Status = true;

            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = bookImage.BookId });

        }

        public IActionResult Edit(int? id)
        {
            if (id is null)
                return NotFound();
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .Include(b => b.BookImages)
                .FirstOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();
            book.TagIds =book.BookTags.Select(b => b.Id).ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid)
                return View(book);

            var existBook = _context.Books.Include(b => b.BookTags).Include(b => b.BookImages).FirstOrDefault(b => b.Id == book.Id);
            if (existBook == null)
                return NotFound();

            if (!_context.Genres.Any(g => g.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre not found");
                return View(book);
            }
            if (!_context.Authors.Any(a => a.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author not found");
                return View(book);
            }

            existBook.Name = book.Name;
            existBook.Description = book.Description;
            existBook.SalePrice = book.SalePrice;
            existBook.CostPrice = book.CostPrice;
            existBook.InStock = book.InStock;
            existBook.IsNew = book.IsNew;
            existBook.IsFeatured = book.IsFeatured;
            existBook.Rate = book.Rate;
            existBook.GenreId = book.GenreId;
            existBook.AuthorId = book.AuthorId;

            existBook.BookTags.Clear();
            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(t => t.Id == tagId))
                {
                    ModelState.AddModelError("TagIds", "TagId not found");
                    return View(book);
                }
                existBook.BookTags.Add(new BookTag { TagId = tagId, BookId = existBook.Id });
            }

            if (book.Photos != null && book.Photos.Count > 0)
            {
                foreach (var file in book.Photos)
                {

                    BookImage bookImage = new()
                    {
                        Name = file.SaveImage(_env.WebRootPath, "assets/image/products"),
                        Status = (existBook.BookImages.Count == 0)
                    };
                    existBook.BookImages.Add(bookImage);
                }
            }

            _context.Books.Update(existBook);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
