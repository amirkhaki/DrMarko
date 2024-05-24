using DrMarko.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrMarko.ViewComponents;

public class PostsViewComponent : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(Product product)
	{
		return View(product);
	}
}
