using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrMarko.Data;
using DrMarko.Models;
using DrMarko.Areas.Admin.Models.ViewModels;
using Minio;
using System.Security.Cryptography;
using Minio.DataModel.Args;

namespace DrMarko.Areas.Admin.Controllers;

[Area("Admin")]
public class SlidersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;

    public SlidersController(ApplicationDbContext context, IMinioClient minioClient, IConfiguration configuration)
    {
        _context = context;
        _minioClient = minioClient;
        _configuration = configuration;
    }

    // GET: Admin/Sliders
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Slider.Include(s => s.Image);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Admin/Sliders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slider = await _context.Slider
            .Include(s => s.Image)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (slider == null)
        {
            return NotFound();
        }

        return View(slider);
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

    // GET: Admin/Sliders/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Sliders/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SliderViewModel sliderVM)
    {
        if (!ModelState.IsValid)
        {
            return View(sliderVM);
        }
        var categoryViewModel = sliderVM;
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
        var slider = new Slider 
        {
            SubTitle = sliderVM.SubTitle,
            Title = sliderVM.Title,
            Alignment = sliderVM.Alignment,
            Url = sliderVM.Url,
            ImageId = image.Id,
            ButtonText = sliderVM.ButtonText,
        };
        _context.Add(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Sliders/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slider = await _context.Slider.FindAsync(id);
        if (slider == null)
        {
            return NotFound();
        }
        ViewData["ImageId"] = new SelectList(_context.Image, "Id", "Id", slider.ImageId);
        return View(slider);
    }

    // POST: Admin/Sliders/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,SubTitle,Title,Alignment,ImageId,Url,ButtonText")] Slider slider)
    {
        if (id != slider.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(slider);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SliderExists(slider.Id))
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
        ViewData["ImageId"] = new SelectList(_context.Image, "Id", "Id", slider.ImageId);
        return View(slider);
    }

    // GET: Admin/Sliders/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var slider = await _context.Slider
            .Include(s => s.Image)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (slider == null)
        {
            return NotFound();
        }

        return View(slider);
    }

    // POST: Admin/Sliders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var slider = await _context.Slider.FindAsync(id);
        if (slider != null)
        {
            _context.Slider.Remove(slider);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SliderExists(int id)
    {
        return _context.Slider.Any(e => e.Id == id);
    }
}

