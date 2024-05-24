using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DrMarko.Models;
using DrMarko.Data;
using Microsoft.EntityFrameworkCore;
using DrMarko.Models.ViewModels;

namespace DrMarko.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // TODO: create a view model to include categories
    public async Task<IActionResult> Index()
    {
        var viewModel = new IndexViewModel
        {
            Products = await _context.Product
                .Include(p => p.Images)
                .ToListAsync(),
            Categories = await _context.Category.ToListAsync(),
            Sliders = await _context.Slider.ToListAsync(),
        };
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
