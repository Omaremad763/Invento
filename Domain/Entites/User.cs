using Microsoft.AspNetCore.Identity;

namespace Domain.Entites
{
    public class User : IdentityUser<Guid>
    {
        public bool IsDeleted { get; set; }
    }
}