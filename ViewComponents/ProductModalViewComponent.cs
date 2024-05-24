using DrMarko.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrMarko.ViewComponents;

public class ProductModalViewComponent : ViewComponent
{
	public IViewComponentResult Invoke(Product product)
	{
		return View(product);
	}
}
