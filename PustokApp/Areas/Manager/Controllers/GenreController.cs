using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Helpers;
using PustokApp.Models;

namespace PustokApp.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize]

    public class GenreController : Controller
    {
        private readonly PustokDbContext _context;

        public GenreController(PustokDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int take = 2)
        {
            var query = _context.Genres.Include(g => g.Books).AsQueryable();
            PaginatedList<Genre> paginatedList = PaginatedList<Genre>.Create(query, take, page);
            return View(paginatedList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid) 
            {
                return View();
            }
            if(_context.Genres.Any(g => g.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "This genre already exist");
                return View();
            }
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int?id)
        {
            if (id is null) return NotFound();
            Genre genre = _context.Genres.Find(id);
            if (genre is null) return NotFound();

            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            if (_context.Genres.Any(g => g.Name == genre.Name&& g.Id!=genre.Id))
            {
                ModelState.AddModelError("Name", "This genre already exist");
                return View();
            }
            Genre existGenre = _context.Genres.Find(genre.Id);
            if (genre is null) return NotFound();

            existGenre.Name = genre.Name;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();

            Genre genre = _context.Genres.Find(id);
            if (genre is null) return NotFound();

            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return Ok();
        }

        public IActionResult Detail(int? id)
        {
            if (id is null) return NotFound();
            Genre genre = _context.Genres.Include(c=>c.Books).FirstOrDefault(g=>g.Id==id);
            if (genre is null) return NotFound();

            return View(genre);
        }

    }
}
