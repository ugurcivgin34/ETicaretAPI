using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities.Identy
{
    public class AppUser : IdentityUser<string>
    {
        public string NameSurname { get; set; }

    }
}
