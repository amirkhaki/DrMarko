using System.ComponentModel.DataAnnotations;

namespace DrMarko.Models;

public class ProductImage
{
	public int Id { get; set; }

	public string Alt { get; set; } = string.Empty;

	public int ImageId { get; set; }
	public Image? Image { get; set; }

	public int ProductId { get; set; }
	public Product? Product { get; set; }
}

public class Image
{
	public int Id { get; set; }
	[StringLength(64)]
	public string Hash { get; set; } = string.Empty;

	public ICollection<ProductImage> ProductImages { get; set; } =
		new List<ProductImage>();
	public ICollection<Category> Categories { get; set; } =
		new List<Category>();
}
