namespace Core.Entities;

public class Department:BaseEntity
{
    public int DepartmentId { get; set; }
    public string Name { get; set; }

    public ICollection<Employee> Employees { get; set; }
}