using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DrMarko.Models;
namespace DrMarko.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<DrMarko.Models.Product> Product { get; set; } = default!;

public DbSet<DrMarko.Models.ProductImage> ProductImage { get; set; } = default!;

public DbSet<DrMarko.Models.Image> Image { get; set; } = default!;

public DbSet<DrMarko.Models.Category> Category { get; set; } = default!;

public DbSet<DrMarko.Models.Slider> Slider { get; set; } = default!;

public DbSet<DrMarko.Models.Menu> Menu { get; set; } = default!;
}
