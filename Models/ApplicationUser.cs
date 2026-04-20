using Microsoft.AspNet.Identity.EntityFramework;

public class ApplicationUser : IdentityUser
{
    public int Year { get; set; }
    public ApplicationUser()
    {
    }
}