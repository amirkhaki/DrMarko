using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrMarko.Data;
using DrMarko.Models;
using Microsoft.AspNetCore.Authorization;
using DrMarko.Areas.Admin.Models.ViewModels;

namespace DrMarko.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(m => m.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            List<Category> items = await _context.Category.ToListAsync();
            ViewBag.Categories = new MultiSelectList(items, "Id", "Title");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // TODO: add selecting categories
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var productModel = new Product
            {
                Title = product.Title,
                Description = product.Description,
                Categories = product.Categories
                    .Select(i =>
                    {
                        var category = new Category { Id = i };
                        _context.Attach(category);
                        return category;
                    })
                    .ToList(),
            };
            _context.Add(productModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            List<int> selectedValues = product.Categories.Select(c => c.Id).ToList();
            Console.WriteLine(string.Join(", ", selectedValues));
            MultiSelectList multiSelectList = new MultiSelectList(
                            await _context.Category.ToListAsync(),
                            "Id", "Title", selectedValues);
            multiSelectList.ToList().ForEach(item =>
            {
                Console.WriteLine(item.Selected);
            });
            ViewBag.Categories = multiSelectList;
            var productVM = new ProductViewModel
            {
                Title = product.Title,
                Description = product.Description,
            };
            return View(productVM);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel productVM)
        {

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            try
            {
                var product = await _context.Product
                    .Include(p => p.Categories)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (product == null)
                {
                    return NotFound();
                }
                product.Id = id;
                product.Title = productVM.Title;
                product.Description = productVM.Description;
                product.Categories
                    .Where(c => !productVM.Categories.Any(id => id == c.Id))
                    .ToList()
                    .ForEach(c => product.Categories.Remove(c));
                productVM.Categories
                    .Where(id => !product.Categories.Any(c => c.Id == id))
                    .ToList()
                    .ForEach(id =>
                    {
                        Category item = new Category { Id = id };
                        _context.Attach(item);
                        product.Categories.Add(item);
                    });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
