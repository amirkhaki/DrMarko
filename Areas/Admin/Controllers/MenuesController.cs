using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrMarko.Data;
using DrMarko.Models;

namespace DrMarko.Areas.Admin.Controllers;

[Area("Admin")]
public class MenuesController : Controller
{
    private readonly ApplicationDbContext _context;

    public MenuesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Menues
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Menu.Include(m => m.Parent);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Admin/Menues/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menu = await _context.Menu
            .Include(m => m.Parent)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (menu == null)
        {
            return NotFound();
        }

        return View(menu);
    }

    // GET: Admin/Menues/Create
    public IActionResult Create()
    {
        ViewData["ParentId"] = new SelectList(_context.Menu, "Id", "Id");
        return View();
    }

    // POST: Admin/Menues/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Text,Url,Priority,ParentId")] Menu menu)
    {
        if (ModelState.IsValid)
        {
            _context.Add(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ParentId"] = new SelectList(_context.Menu, "Id", "Id", menu.ParentId);
        return View(menu);
    }

    // GET: Admin/Menues/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menu = await _context.Menu.FindAsync(id);
        if (menu == null)
        {
            return NotFound();
        }
        ViewData["ParentId"] = new SelectList(_context.Menu, "Id", "Id", menu.ParentId);
        return View(menu);
    }

    // POST: Admin/Menues/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Text,Url,Priority,ParentId")] Menu menu)
    {
        if (id != menu.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(menu);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(menu.Id))
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
        ViewData["ParentId"] = new SelectList(_context.Menu, "Id", "Id", menu.ParentId);
        return View(menu);
    }

    // GET: Admin/Menues/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var menu = await _context.Menu
            .Include(m => m.Parent)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (menu == null)
        {
            return NotFound();
        }

        return View(menu);
    }

    // POST: Admin/Menues/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var menu = await _context.Menu.FindAsync(id);
        if (menu != null)
        {
            _context.Menu.Remove(menu);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MenuExists(int id)
    {
        return _context.Menu.Any(e => e.Id == id);
    }
}
