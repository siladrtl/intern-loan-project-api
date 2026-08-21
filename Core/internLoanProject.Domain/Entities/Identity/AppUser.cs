using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities.Identity
{
    public class AppUser: IdentityUser<Guid>
    {
        public int? CustomerId { get; set; }

        public Customer? Customer { get; set; }
    }
}
