using DemoWUITGallery.Database;
using DemoWUITGallery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoWUITGallery.Controllers
{
    public class PhotoGalleriesController : Controller
    {
        private readonly PhotoGalleryContext _context;

        public PhotoGalleriesController(PhotoGalleryContext context)
        {
            _context = context;
        }

        // GET: PhotoGalleries
        public async Task<IActionResult> Index()
        {

            return View(await _context.PhotoGalleries.ToListAsync());
        }

        // GET: PhotoGalleries/Details/5
        public async Task<IActionResult> Details(int? id, int page = 1, int pageSize = 6)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.galleryId = id;

            PhotoGallery? photoGallery = await _context.PhotoGalleries.FirstOrDefaultAsync(m => m.Id == id);
            if (photoGallery == null)
            {
                return NotFound();
            }


            photoGallery.Images.Content = await _context.Images.Where(x => x.PhotoGalleryId == id)
                .OrderByDescending(x => x.CreationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            photoGallery.Images.TotalRecords = _context.Images.Where(x => x.PhotoGalleryId == id).Count();

            photoGallery.Images.CurrentPage = page;
            photoGallery.Images.PageSize = pageSize;

            if (page > 1)
            {
                photoGallery.Images.HasPreviousPage = true;
            }

            if (page * pageSize < photoGallery.Images.TotalRecords)
            {
                photoGallery.Images.HasNextPage = true;
            }

            return View(photoGallery);
        }

        // GET: PhotoGalleries/ImagesDetails/5
        public async Task<IActionResult> ImagesDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PhotoGallery? photoGallery = await _context.PhotoGalleries.FirstOrDefaultAsync(m => m.Id == id);

            if (photoGallery == null)
            {
                return NotFound();
            }

            List<Models.Image> imageList = await _context.Images.Where(x => x.PhotoGalleryId == id)
                .OrderByDescending(x => x.CreationDate).ToListAsync();

            return PartialView("_ImageCarousel", imageList);
        }

        // GET: PhotoGalleries/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] PhotoGallery photoGallery)
        {
            photoGallery.CreationDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(photoGallery);
                await _context.SaveChangesAsync();
                TempData["Success"] = photoGallery.Title + " successfully created";
                return RedirectToAction(nameof(Index));
            }
            return View(photoGallery);
        }

        // GET: PhotoGalleries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PhotoGallery? photoGallery = await _context.PhotoGalleries.FindAsync(id);

            if (photoGallery == null)
            {
                return NotFound();
            }

            ViewBag.galleryId = photoGallery.Id;
            return View(photoGallery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] PhotoGallery photoGallery)
        {
            if (id != photoGallery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photoGallery);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = photoGallery.Title + " is successfully updated!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoGalleryExists(photoGallery.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(photoGallery);
        }

        // GET: PhotoGalleries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PhotoGallery? photoGallery = await _context.PhotoGalleries.FirstOrDefaultAsync(m => m.Id == id);

            return photoGallery == null ? NotFound() : View(photoGallery);
        }

        // POST: PhotoGalleries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            PhotoGallery? photoGallery = await _context.PhotoGalleries.FindAsync(id);

            if (photoGallery != null&& (photoGallery.Id != 0))
            {
                _context.PhotoGalleries.Remove(photoGallery);
                await _context.SaveChangesAsync();
                TempData["Success"] = photoGallery.Title + " is removed succesfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Photo gallery id is null";
                return RedirectToAction("Index");
            }            
        }

        // return image modal
        public IActionResult GetImageModal()
        {
            return PartialView("_UploadImage");
        }

        private bool PhotoGalleryExists(int id)
        {
            return _context.PhotoGalleries.Any(e => e.Id == id);
        }
    }
}
