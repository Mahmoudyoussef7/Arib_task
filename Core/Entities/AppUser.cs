using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Identity;

public class AppUser : IdentityUser
{
    public int? EmployeeId { get; set; }
    public Employee Employee { get; set; }
}


