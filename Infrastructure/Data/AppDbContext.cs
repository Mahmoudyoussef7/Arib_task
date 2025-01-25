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
            .HasOne(e => e.Manager)
            .WithMany(m => m.Subordinates)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.ApplicationUser)
            .WithOne(au => au.Employee)
            .HasForeignKey<Employee>(e => e.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Department>()
            .HasMany(d => d.Employees)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.AssignedToEmployee)
            .WithMany(e => e.Tasks)
            .HasForeignKey(t => t.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Task>()
            .HasOne(t => t.Manager)
            .WithMany()
            .HasForeignKey(t => t.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
