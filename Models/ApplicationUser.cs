using Microsoft.AspNetCore.Identity;

namespace DrMarko.Models;

public class ApplicationUser : IdentityUser
{
    public Cart? Cart { get; set; }
}
