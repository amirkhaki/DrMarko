using System.ComponentModel.DataAnnotations;
using DrMarko.Models;

namespace DrMarko.Areas.Admin.Models.ViewModels;

public class SliderViewModel
{
    public int Id { get; set; }
    public string SubTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    [Required]
    public SliderAlignment Alignment { get; set; }
    [Required]
    public IFormFile? Image { get; set; }
    [Url]
    public string Url { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
}
