using DemoWUITGallery.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace DemoWUITGallery.Controllers
{
    public class ImagesController : Controller
    {
        private readonly PhotoGalleryContext _context;
        private readonly IWebHostEnvironment Environment;

        public ImagesController(PhotoGalleryContext context, IWebHostEnvironment _environment)
        {
            _context = context;
            Environment = _environment;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Images.Include(x=>x.PhotoGallery).ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.Image? image = await _context.Images.FirstOrDefaultAsync(m => m.Id == id);
            return image == null ? NotFound() : View(image);
        }

        // GET: Images/Create
        public ActionResult Create(int? PhotogalleryId)
        {
            Models.Image image = new()
            {
                PhotoGalleryId = (int?)PhotogalleryId ?? 0,
                CreationDate = DateTime.Now
            };
            if(PhotogalleryId==0)
                ViewBag.PhotoGalleryId = _context.PhotoGalleries.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToList();
            return PartialView("_UploadImage", image);
        }

        // POST: Images/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhotoGalleryId,Title,Cdn_path,CreationDate")] Models.Image image)
        {
            image.CreationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(image);
                await _context.SaveChangesAsync();
                TempData["Success"] = image.Title + " is successfully created!";
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.Image? image = await _context.Images.FindAsync(id);
            ViewBag.PhotoGalleryId = await _context.PhotoGalleries.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Title }).ToListAsync();
            return image == null ? NotFound() : View(image);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadImage(Models.Image image, IFormFileCollection files)
        {

            if (files.Count() == 0 || files.FirstOrDefault() == null)
            {
                ModelState.AddModelError(string.Empty, "You need  to upload file");
                return PartialView("_UploadImage", image);
            }

            Models.Image model = new();
            foreach (IFormFile file in files)
            {
                try
                {
                    if (file.Length == 0) { continue; }

                    model.Title = image.Title;
                    model.PhotoGalleryId = image.PhotoGalleryId;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file.FileName).ToLower();

                    using SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());
                    string name = string.Format("{0}{1}", fileName, extension).ToString();
                    model.ThumbnailPath = Path.Combine("/GalleryImages/Thumbs/", name);
                    model.ImagePath = Path.Combine("/GalleryImages/", name);

                    System.Drawing.Size imageSize = new(img.Width, img.Height);
                    System.Drawing.Size thumbSize = NewSize(imageSize, new System.Drawing.Size(100, 100));


                    if (!ModelState.IsValid)
                    {
                        return PartialView("_UploadImage", model);
                    }

                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    string wwwpath = Environment.WebRootPath.ToString();
                    string thumbPath = string.Concat(wwwpath, model.ThumbnailPath.ToString());
                    string imagePath = string.Concat(wwwpath, model.ImagePath.ToString());
                    img.Save(imagePath);
                    img.Mutate(x => x.Resize(thumbSize.Width, thumbSize.Height));
                    img.Save(thumbPath);
                    TempData["Success"] = model.Title + " is uploaded";

                }
                catch (Exception)
                {

                }
            }
            return PartialView("_UploadImage", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Image image)
        {
            if (id != image.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = image.Title + " is successfully updated!"; 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.Image? image = await _context.Images.FirstOrDefaultAsync(m => m.Id == id);
            return image == null ? NotFound() : View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Models.Image? image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
                TempData["Success"] = image.Title + " is deleted!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.Id == id);
        }

        private System.Drawing.Size NewSize(System.Drawing.Size imageSize, System.Drawing.Size newSize)
        {
            System.Drawing.Size finalSize;
            double tempval;

            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                tempval = imageSize.Height > imageSize.Width ? newSize.Height / (imageSize.Height * 1.0) : newSize.Width / (imageSize.Width * 1.0);
                finalSize = new System.Drawing.Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
            {
                finalSize = imageSize;
            }
            return finalSize;
        }
    }
}
