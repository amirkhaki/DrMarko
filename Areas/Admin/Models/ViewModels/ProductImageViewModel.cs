using System.ComponentModel.DataAnnotations;

namespace DrMarko.Areas.Admin.Models.ViewModels;

public class ProductImageViewModel
{
    public string Alt { get; set; } = string.Empty;
    [Required]
    public IFormFile? Image { get; set; }
}
