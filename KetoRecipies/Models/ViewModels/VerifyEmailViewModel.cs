using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KetoRecipies.Models.ViewModels
{
    public class VerifyEmailViewModel
    {
        public string JSONUser { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int VerificationCode { get; set; }
    }
}
