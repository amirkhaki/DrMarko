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
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
    {
        var AddedEntities = ChangeTracker.Entries<SaveConfig>().Where(E => E.State == EntityState.Added).ToList();

        AddedEntities.ForEach(E =>
        {
            E.Entity.CreationTime = DateTime.Now;
            E.Entity.ModifiedDate = E.Entity.CreationTime;
        });

        var EditedEntities = ChangeTracker.Entries<SaveConfig>().Where(E => E.State == EntityState.Modified).ToList();

        EditedEntities.ForEach(E =>
        {
            E.Entity.ModifiedDate = DateTime.Now;
        });

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public DbSet<DrMarko.Models.Product> Product { get; set; } = default!;

    public DbSet<DrMarko.Models.ProductImage> ProductImage { get; set; } = default!;

    public DbSet<DrMarko.Models.Image> Image { get; set; } = default!;

    public DbSet<DrMarko.Models.Category> Category { get; set; } = default!;

    public DbSet<DrMarko.Models.Slider> Slider { get; set; } = default!;

    public DbSet<DrMarko.Models.Menu> Menu { get; set; } = default!;
    public DbSet<DrMarko.Models.Cart> Cart { get; set; } = default!;
    public DbSet<DrMarko.Models.CartEntry> CartEntry { get; set; } = default!;
}
