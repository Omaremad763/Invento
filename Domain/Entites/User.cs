using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace Domain.Entites
{
    public class User : IdentityUser<Guid>
    {
        public bool IsDeleted { get;  set; }
    }
}
