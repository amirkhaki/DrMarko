using DrMarko.Data;
using DrMarko.Models;
using Htmx;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrMarko.Controllers;

public class CartController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
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
        return Request.IsHtmx() ? ViewComponent("Cart") : View(user.Cart);
    }

    public async Task<IActionResult> AddItem(int productId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            // user is not logged in
            // TODO add cart to cookie
            return Unauthorized();
        }
        var user = await GetUserWithCart(userId);
        var entry = user.Cart!.Entries.FirstOrDefault(e => e.ProductId == productId);

        if (entry is null)
        {
            entry = new CartEntry
            {
                CartId = user.Cart!.Id,
                ProductId = productId,
                Quantity = 0,
            };
            user.Cart.Entries.Add(entry);
        }
        entry.Quantity += 1;
        await _context.SaveChangesAsync();
        Response.Htmx(h =>
        {
            h.WithTrigger("cart-added", timing: HtmxTriggerTiming.AfterSwap);
        });
        return ViewComponent("Cart");
    }
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
        {
            // user is not logged in
            // TODO add cart to cookie
            return Unauthorized();
        }
        var user = await GetUserWithCart(userId);
        // get product from cart and remove it
        var item = user.Cart!.Entries.FirstOrDefault(e => e.ProductId == productId);
        if (item is null)
        {
            return NotFound();
        }
        user.Cart.Entries.Remove(item);
        await _context.SaveChangesAsync();
        if (Request.IsHtmx(out var values))
        {
            Response.Htmx(h =>
            {
                h.WithTrigger("cart-removed", timing: HtmxTriggerTiming.AfterSwap);
            });
            return ViewComponent("Cart", values!.Target == "cart-table");
        }
        return ViewComponent("Cart");
    }

    private async Task<ApplicationUser> GetUserWithCart(string? userId)
    {
        var user = await _context.Users
            .Include(u => u.Cart)
            .ThenInclude(c => c!.Entries)
            .SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            // unexpected
            throw new InvalidDataException("user logged in but no such user in db");
        }
        if (user!.Cart is null)
        {
            user!.Cart = new Cart
            {
                UserId = user.Id,
            };
            await _context.SaveChangesAsync();
        }
        return user;
    }
}
