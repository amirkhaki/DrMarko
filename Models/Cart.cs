namespace DrMarko.Models;

public class Cart
{
    public int Id { get; set; }
    public virtual required string UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<CartEntry> Entries { get; set; } = null!;
}
