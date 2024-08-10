using DrMarko.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrMarko.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(int first_id = 1, int size = 10)
    {
        var model = await _context.Category
            .Where(c => c.Id >= first_id)
            .Take(size)
            .ToListAsync();
        return base.View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _context.Category
                    .Include(c => c.Products)
                    .ThenInclude(p => p.Images)
                    .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category.Products);
    }
}
