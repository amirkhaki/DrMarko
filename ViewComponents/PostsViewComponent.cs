using DrMarko.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrMarko.ViewComponents;

public class PostsViewComponent : ViewComponent
{
	public IViewComponentResult Invoke(Product product)
	{
		return View(product);
	}
}
