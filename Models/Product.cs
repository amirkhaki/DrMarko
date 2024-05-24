namespace DrMarko.Models;

public class Product
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
	public ICollection<Category> Categories { get; set; } =
		new List<Category>();
}
