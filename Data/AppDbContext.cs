using M01.BaselineAPIProjectController.Data.Configurations;
using M01.BaselineAPIProjectController.Entities;
using Microsoft.EntityFrameworkCore;

namespace M01.BaselineAPIProjectController.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectConfiguration).Assembly);
    }
}