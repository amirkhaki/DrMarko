namespace DrMarko.Models;

public enum CartEntryStatus
{
    Canceled,
    InProgress,
    Complete
}
public class CartEntry : SaveConfig
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public CartEntryStatus Status { get; set; }
}
