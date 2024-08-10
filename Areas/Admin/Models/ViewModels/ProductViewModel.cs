namespace DrMarko.Areas.Admin.Models.ViewModels;

public class ProductViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<int> Categories { get; set; } = new List<int>();
}
