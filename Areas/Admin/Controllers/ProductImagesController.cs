using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrMarko.Data;
using DrMarko.Models;
using DrMarko.Areas.Admin.Models.ViewModels;
using System.Security.Cryptography;
using Minio;
using Minio.DataModel.Args;
using Microsoft.AspNetCore.Authorization;

namespace DrMarko.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class ProductImagesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;

    // TODO: break lines to be readable in split pane

    public ProductImagesController(ApplicationDbContext context,
        IMinioClient minioClient, IConfiguration configuration)
    {
        _context = context;
        _minioClient = minioClient;
        _configuration = configuration;
    }

    // GET: Admin/ProductImages
    [Route("Admin/Products/{pid:int}/ProductImages")]
    public async Task<IActionResult> Index(int pid)
    {
        var applicationDbContext = _context.ProductImage
            .Include(p => p.Image)
            .Include(p => p.Product)
            .Where(pi => pi.ProductId == pid);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Admin/ProductImages/Details/5
    [Route("Admin/Products/{pid:int}/ProductImages/Details/{id}")]
    public async Task<IActionResult> Details(int pid, int id)
    {
        var productImage = await _context.ProductImage
            .Include(p => p.Image)
            .Include(p => p.Product).Where(pi => pi.ProductId == pid)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (productImage == null)
        {
            return NotFound();
        }

        return View(productImage);
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

    // GET: Admin/ProductImages/Create
    [Route("Admin/Products/{pid:int}/ProductImages/Create")]
    public IActionResult Create(int pid)
    {
        return View();
    }

    // POST: Admin/ProductImages/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [Route("Admin/Products/{pid:int}/ProductImages/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int pid,
        ProductImageViewModel productImage)
    {
        if (!_context.Product.Any(e => e.Id == pid))
        {
            return NotFound();
        }
        if (!ModelState.IsValid)
        {
            return View(productImage);
        }

        var file = productImage.Image!;
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
        var pImage = new ProductImage
        {
            ProductId = pid,
            Alt = productImage.Alt,
            ImageId = image.Id,
        };
        _context.Add(pImage);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { pid = pid });
    }

    // GET: Admin/ProductImages/Edit/5
    [Route("Admin/Products/{pid:int}/ProductImages/Edit/{id}")]
    public async Task<IActionResult> Edit(int pid, int id)
    {

        var productImage = await _context.ProductImage
            .Where(pi => pi.ProductId == pid).FirstOrDefaultAsync(pi => pi.Id == id);
        if (productImage == null)
        {
            return NotFound();
        }
        ViewData["ImageId"] = new SelectList(_context.Set<Image>(), "Id",
            "Id", productImage.ImageId);
        ViewData["ProductId"] = new SelectList(_context.Product, "Id", 
            "Id", productImage.ProductId);
        return View(productImage);
    }

    // POST: Admin/ProductImages/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [Route("Admin/Products/{pid:int}/ProductImages/Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int pid, int id, 
        [Bind("Id,Alt,ImageId,ProductId")] ProductImage productImage)
    {
        if (id != productImage.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(productImage);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductImageExists(productImage.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index), new { pid = pid });
        }
        ViewData["ImageId"] = new SelectList(_context.Set<Image>(), "Id", 
            "Id", productImage.ImageId);
        ViewData["ProductId"] = new SelectList(_context.Product, "Id", 
            "Id", productImage.ProductId);
        return View(productImage);
    }

    // GET: Admin/ProductImages/Delete/5
    [Route("Admin/Products/{pid:int}/ProductImages/Delete/{id}")]
    public async Task<IActionResult> Delete(int pid, int id)
    {

        var productImage = await _context.ProductImage
            .Include(p => p.Image)
            .Include(p => p.Product)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (productImage == null)
        {
            return NotFound();
        }

        return View(productImage);
    }

    // POST: Admin/ProductImages/Delete/5
    [HttpPost, ActionName("Delete")]
    [Route("Admin/Products/{pid:int}/ProductImages/Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int pid, int id)
    {
        var productImage = await _context.ProductImage.FindAsync(id);
        if (productImage != null)
        {
            _context.ProductImage.Remove(productImage);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { pid = pid });
    }

    private bool ProductImageExists(int id)
    {
        return _context.ProductImage.Any(e => e.Id == id);
    }
}

