using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;
using PustokApp.Services;
using PustokApp.ViewModels;
using System.Linq;

namespace PustokApp.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokDbContext _pustokDbContext;
        private readonly BankAccountService _bankAccountService;
        private readonly UserManager<AppUser> _userManager;

        public BookController(PustokDbContext pustokDbContext, BankAccountService bankAccountService, Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager)
        {
            _pustokDbContext = pustokDbContext;
            _bankAccountService = bankAccountService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }    
        public IActionResult Detail(int?id)
        {
            if (id is null)
                return NotFound();
            var existBook = _pustokDbContext.Books
                .Include(b=>b.Author)
                .Include(b=>b.Genre)
                .Include(b=>b.BookTags).ThenInclude(bc => bc.Tag)
                .Include(b=>b.BookImages)
                .Include(b=>b.BookComments)
                .ThenInclude(bc=>bc.AppUser)
                .FirstOrDefault(x => x.Id == id);
            if (existBook is null)
                return NotFound();
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null || _userManager.IsInRoleAsync(user, "member").Result)
            {
                return RedirectToAction("login", "account", new {returnUrl = Url.Action("detail", "book", id=id)});

            }

            return View("detail", getBookDetailVm(existBook.Id, user.Id));

        }

        [HttpPost]
        public async Task<IActionResult> AddComment(BookComment bookComment)
        {
            if (!ModelState.IsValid)  return RedirectToAction("Detail", new {id=bookComment.BookId});
            if(_pustokDbContext.Books.Any(b=>b.Id== bookComment.Id))
            {
                return RedirectToAction("NotFound", "Error");

            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "member") || _pustokDbContext.BookComments.Any(x => x.BookId == bookComment.BookId && x.AppUserId == user.Id && x.Status != CommentStatus.Rejected))
            {
                return RedirectToAction("NotFound", "Error");
            }

            bookComment.AppUserId = user.Id;
            _pustokDbContext.BookComments.Add(bookComment);
            await _pustokDbContext.SaveChangesAsync();

            return View("detail", getBookDetailVm(bookComment.BookId, user.Id));
        }
        private BookDetailVM getBookDetailVm(int bookId, string userId)
        {
            var existBook = _pustokDbContext.Books
              .Include(b => b.Author)
              .Include(b => b.Genre)
              .Include(b => b.BookTags).ThenInclude(bc => bc.Tag)
              .Include(b => b.BookImages)
              .Include(b => b.BookComments)
              .ThenInclude(bc => bc.AppUser)
              .FirstOrDefault(x => x.Id == bookId);

            BookDetailVM bookDetailVm = new BookDetailVM
            {
                Book = existBook,
                RelatedBooks = _pustokDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Where(x => x.GenreId == existBook.GenreId && x.Id != existBook.Id)
                .Take(5)
                .ToList()

            };
            bookDetailVm.TotalComments = _pustokDbContext.BookComments.Count(x => x.BookId == existBook.Id);
            bookDetailVm.AvgRate = bookDetailVm.TotalComments > 0 ? (int)_pustokDbContext.BookComments.Where(x => x.BookId == existBook.Id).Average(x => x.Rate) : 0;
            return bookDetailVm;
        }

    }
}
