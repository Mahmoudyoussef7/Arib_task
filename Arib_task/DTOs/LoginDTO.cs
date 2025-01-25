using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Arib_task.DTOs;

public class LoginDTO
{

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; }
}
