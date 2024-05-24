namespace DrMarko.Models.ViewModels;

public class IndexViewModel
{
	public IEnumerable<Product> Products { get; set; } =
		new List<Product>();
	public IEnumerable<Category> Categories { get; set; } =
		new List<Category>();
	public IEnumerable<Slider> Sliders { get; set; } =
		new List<Slider>();
	public IEnumerable<Menu> Menus { get; set; } =
		new List<Menu>();
}
