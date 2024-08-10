using System.ComponentModel.DataAnnotations;

namespace DrMarko.Areas.Admin.Models.ViewModels;

public class CategoryViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required]
    public IFormFile? Image { get; set; }
}
