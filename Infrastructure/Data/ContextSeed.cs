using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Data;
public static class ContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Departments.Any())
        {
            var departmentData = File.ReadAllText("../Infrastructure/DataSeed/Departments.json");
            var departments = JsonSerializer.Deserialize<List<Department>>(departmentData);
            await context.Departments.AddRangeAsync(departments);
        }

        if (!context.Employees.Any())
        {
            var employeesData = File.ReadAllText("../Infrastructure/DataSeed/Employees.json");
            var employees = JsonSerializer.Deserialize<List<Employee>>(employeesData);
            await context.Employees.AddRangeAsync(employees);
        }

        if (!context.Tasks.Any())
        {
            var tasksData = File.ReadAllText("../Infrastructure/Data/SeedData/Tasks.json");
            var tasks = JsonSerializer.Deserialize<List<Core.Entities.Task>>(tasksData);
            await context.Tasks.AddRangeAsync(tasks);
        }

        if (context.ChangeTracker.HasChanges())
            await context.SaveChangesAsync();
    }
}
