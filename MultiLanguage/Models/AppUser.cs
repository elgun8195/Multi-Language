using Microsoft.AspNetCore.Identity;

namespace MultiLanguage.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; } 
    }
}
