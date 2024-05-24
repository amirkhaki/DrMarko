using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrMarko.Data;
using DrMarko.Models;
using Minio;
using System.Security.Cryptography;
using DrMarko.Areas.Admin.Models.ViewModels;
using Minio.DataModel.Args;

namespace DrMarko.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;

    public CategoriesController(ApplicationDbContext context,
        IMinioClient minioClient, IConfiguration configuration)
    {
        _context = context;
        _minioClient = minioClient;
        _configuration = configuration;
    }

    private string fileHash(byte[] file)
    {
        var hash = SHA256.Create().ComputeHash(file);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }
    // GET: Admin/Categories
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Category.Include(c => c.Image);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Admin/Categories/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Category
            .Include(c => c.Image)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: Admin/Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Categories/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(categoryViewModel);
        }
        var file = categoryViewModel.Image!;
        using var fileStream = new MemoryStream();
        file.CopyTo(fileStream);
        var fileBytes = fileStream.ToArray();
        var imageHash = fileHash(fileBytes);

        var image = await _context.Image
            .SingleOrDefaultAsync(i => i.Hash == imageHash);
        if (image is null)
        {
            // create image and upload it
            var bucketName = _configuration["Minio:ImagesBucket"];
            var contentType = file.ContentType;

            var putObjectArgs = new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(imageHash)
                            .WithStreamData(new MemoryStream(fileBytes))
                            .WithObjectSize(fileStream.Length)
                            .WithContentType(contentType);
            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            image = new Image
            {
                Hash = imageHash,
            };
            _context.Image.Add(image);
            await _context.SaveChangesAsync();
        }
        var category = new Category
        {
            Title = categoryViewModel.Title,
            Description = categoryViewModel.Description,
            ImageId = image.Id,
        };
        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Category.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        ViewData["ImageId"] = new SelectList(_context.Image, "Id", "Id", category.ImageId);
        return View(category);
    }

    // POST: Admin/Categories/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ImageId")] Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
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
        ViewData["ImageId"] = new SelectList(_context.Image, "Id", "Id", category.ImageId);
        return View(category);
    }

    // GET: Admin/Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Category
            .Include(c => c.Image)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Admin/Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Category.FindAsync(id);
        if (category != null)
        {
            _context.Category.Remove(category);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _context.Category.Any(e => e.Id == id);
    }
}

