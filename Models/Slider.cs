using System.ComponentModel.DataAnnotations;

namespace DrMarko.Models;

public class Slider
{
    public int Id { get; set; }
    public string SubTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    [Required]
    public SliderAlignment Alignment { get; set; }
    public int ImageId { get; set; }
    public Image? Image { get; set; }
    [Url]
    public string Url { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
}

public enum SliderAlignment
{
    Right,
    Left,
    Center
}
