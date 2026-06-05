using Microsoft.EntityFrameworkCore;

public class ProjetoWebContext : DbContext
{
    public ProjetoWebContext(DbContextOptions<ProjetoWebContext> options) : base(options)
    {
    }

    public DbSet<ProjetoWeb.Models.Department> Department { get; set; } = default!;
}