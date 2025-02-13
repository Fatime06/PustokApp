using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PustokApp.Data;
using PustokApp.Helpers;
using PustokApp.Models;
using System.Drawing;

namespace PustokApp.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize]

    public class SliderController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly JwtServiceOption _jwtServiceOption;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(PustokDbContext context, IOptions<JwtServiceOption> options, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _jwtServiceOption = options.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (slider.Photo == null)
            {
                ModelState.AddModelError("Photo", "Photo is required");
            }
            if (!ModelState.IsValid)
                return View();

            var file = slider.Photo;
            slider.Image = file.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/bg-images");
            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("Index"); ;

        }

        public IActionResult Edit(int?id)
        {
            if (id is null)
                return NotFound();
            var slider = _context.Sliders.Find(id);
            if (slider is null)
                return NotFound();
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Slider slider)
        {
            if (!ModelState.IsValid)
                return View(slider);

            var existingSlider = _context.Sliders.Find(id);
            if (existingSlider == null) return NotFound();

            string oldImage = existingSlider.Image;

            if (slider.Photo != null) 
            {
                if (!string.IsNullOrEmpty(oldImage))
                {
                    var deletedImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/bg-images", oldImage);
                    FileManager.DeleteFile(deletedImagePath);
                }

                existingSlider.Image = slider.Photo.SaveImage(_webHostEnvironment.WebRootPath, "assets/image/bg-images");
            }

            existingSlider.Title = slider.Title;
            existingSlider.Description = slider.Description;
            existingSlider.ButtonLink = slider.ButtonLink;
            existingSlider.ButtonText = slider.ButtonText;
            existingSlider.Order = slider.Order;

            _context.Sliders.Update(existingSlider);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var exitSlider = _context.Sliders.Find(id);
            if (exitSlider == null) return NotFound();

            string deletedImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/image/bg-images", exitSlider.Image);
            FileManager.DeleteFile(deletedImagePath);

            _context.Sliders.Remove(exitSlider);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult ReadData() 
        {
            var data1 = _jwtServiceOption.Key;
            var data2 = _jwtServiceOption.Issuer;

            return Json(new
            {
                key = data1,
                issuer = data2
            });

        }

    }
}
