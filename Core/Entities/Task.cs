using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;
public class Task:BaseEntity
{
    public int TaskId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public int AssignedToEmployeeId { get; set; }
    public int ManagerId { get; set; }


    public Employee AssignedToEmployee { get; set; }
    public Employee Manager { get; set; }
}
