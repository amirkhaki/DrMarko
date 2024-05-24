namespace DrMarko.Models;

public class Category
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int ImageId { get; set; }
	public Image? Image { get; set; }
	public ICollection<Product> Products { get; set; } = 
		new List<Product>();
}
