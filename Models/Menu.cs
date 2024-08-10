using System.ComponentModel.DataAnnotations;

namespace DrMarko.Models;

public class Menu
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    [Url]
    public string Url { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int? ParentId { get; set; }
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } =
        new List<Menu>();
}
