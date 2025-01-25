using Core.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;
public class Employee:BaseEntity
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal Salary { get; set; }
    public string ImagePath { get; set; }
    public int? ManagerId { get; set; }
    public int DepartmentId { get; set; }
    public string ApplicationUserId { get; set; }

    public Employee Manager { get; set; }
    public Department Department { get; set; }
    public ICollection<Employee> Subordinates { get; set; }
    public ICollection<Task> Tasks { get; set; }
    public AppUser ApplicationUser { get; set; }
}
