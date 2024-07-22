using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class AppDbContext : DbContext
{
    public DbSet<Task> Tasks { get; set; }
    public DbSet<SubTask> SubTasks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>()
            .HasMany(t => t.SubTasks)
            .WithOne(st => st.Task)
            .HasForeignKey(st => st.TaskId);
    }
}

public class Task
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsComplete { get; set; }
    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
}

public class SubTask
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public Task Task { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsComplete { get; set; }
}
