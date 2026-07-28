using Microsoft.AspNetCore.Identity;

namespace Portal.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
