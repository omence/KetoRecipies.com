using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string Facebook { get; set; }
        public string YouTube { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
    }

    public static class ApplicationRoles
    {
        public const string Member = "Member";
        public const string Admin = "Admin";
    }
}
