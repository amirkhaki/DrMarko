using DrMarko.Data;
using DrMarko.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrMarko.ViewComponents;

public record CartViewComponentData(Cart Cart, bool IncludeTable);
public class CartViewComponent : ViewComponent
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;


    public CartViewComponent(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(bool includeTable = false)
    {
        var userId = _userManager.GetUserId(UserClaimsPrincipal);
        if (userId is null)
        {
            // handle the case for guest users
        }
        var user = await _context.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c!.Entries)
                .ThenInclude(e => e.Product)
                .ThenInclude(p => p.Images)
                .SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            // TODO handle guest users
            return Content("you are not logged in");
        }
        if (user!.Cart is null)
        {
            user!.Cart = new Cart
            {
                UserId = user.Id,
            };
            await _context.SaveChangesAsync();
        }
        var data = new CartViewComponentData(user!.Cart, includeTable);
        return View(data);
    }
}
