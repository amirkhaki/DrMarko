using DrMarko.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrMarko.ViewComponents;

public class NavigationViewComponent : ViewComponent
{
	public readonly ApplicationDbContext _context;

	public NavigationViewComponent(ApplicationDbContext context)
	{
		_context = context;
	}
	public async Task<IViewComponentResult> InvokeAsync()
	{
		var Menus = await _context.Menu
			.Where(m => m.ParentId == null)
			.Include(m => m.Children)
			.ThenInclude(m => m.Children)
			.OrderBy(m => m.Priority)
			.ToListAsync();
		return View(Menus);
	}
}
