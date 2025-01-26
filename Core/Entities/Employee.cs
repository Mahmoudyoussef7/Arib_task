using Core.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;
public class Employee:BaseEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal Salary { get; set; }
    public string? ImagePath { get; set; }
    public int? ManagerId { get; set; }
    public int DepartmentId { get; set; }
    public string AppUserId { get; set; }

    [ForeignKey(nameof(ManagerId))]
    public Employee Manager { get; set; }
    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; }
    public ICollection<Employee> Subordinates { get; set; }
    public ICollection<Task> Tasks { get; set; }
    [ForeignKey(nameof(AppUserId))]
    public AppUser ApplicationUser { get; set; }
}
