using System.ComponentModel.DataAnnotations;

namespace Arib_task.DTOs;

public class EmployeeDTO
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
    public decimal Salary { get; set; }

    [Display(Name = "Image Path")]
    public string ImagePath { get; set; }

    [Display(Name = "Manager Name")]
    public string ManagerName { get; set; }

    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}";
}