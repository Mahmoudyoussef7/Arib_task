using Core.Entities;
using Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task = Core.Entities.Task;


namespace Infrastructure.Data;
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Employee>()
           .HasMany(e => e.Tasks)               
           .WithOne(t => t.AssignedToEmployee)   
           .HasForeignKey(t => t.AssignedToEmployeeId) 
           .OnDelete(DeleteBehavior.Cascade);

    }
}
