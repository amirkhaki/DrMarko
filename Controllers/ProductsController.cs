using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrMarko.Data;

namespace DrMarko.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext contextn)
    {
        _context = contextn;
    }

    // GET: Products
	public async Task<IActionResult> Index(int first_id = 1, int size = 5)
	{
		var model = await _context.Product
			.Where(c => c.Id >= first_id)
			.Take(size)
			.ToListAsync();
		return base.View(model);
	}

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int id, string? name)
    {
        var product = await _context.Product
            .Include(p => p.Images)
            .ThenInclude(i => i.Image)
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
    
    // GET: Products/Modal/5
    public async Task<IActionResult> Modal(int id)
    {
        var product = await _context.Product
            .Include(p => p.Images)
            .ThenInclude(i => i.Image)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return ViewComponent("ProductModal", new
        {
            product = product,
        });
    }
}
