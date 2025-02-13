using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Service;
using PustokApp.Services;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly PustokDbContext _pustokDbContext;
        private readonly BankAccountService _bankAccountService;
        private readonly BankManagerService _bankManagerService;

        public HomeController(PustokDbContext pustokDbContext, BankAccountService bankAccountService, BankManagerService bankManagerService)
        {
            _pustokDbContext = pustokDbContext;
            _bankAccountService = bankAccountService;
            _bankManagerService = bankManagerService;
        }
        public IActionResult Index()
        {
            HomeVM homeVM = new();
            homeVM.Sliders = _pustokDbContext.Sliders.ToList();
            homeVM.NewBooks = _pustokDbContext.Books
                .Include(b=>b.Author)
                .Include(b => b.BookImages.Where(x => x.Status != null))
                .Where(x => x.IsNew).ToList();
            homeVM.FeaturedBooks = _pustokDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.BookImages.Where(x => x.Status != null))
                .Where(x => x.IsFeatured).ToList();       
            homeVM.DiscountBooks = _pustokDbContext.Books
                .Include(b => b.Author)
                .Include(b => b.BookImages.Where(x => x.Status != null))
                .Where(x => x.DiscountPercentage>0).ToList();
            return View(homeVM);
        }

        public IActionResult Add()
        {
            _bankAccountService.AddBalance();
            _bankAccountService.AddBalance();
            _bankAccountService.AddBalance();
            _bankManagerService.Add();
            return Ok(_bankAccountService.Balance);
        }

    }
}
