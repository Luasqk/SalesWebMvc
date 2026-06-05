using Microsoft.EntityFrameworkCore;

namespace ProjetoWeb.Data
{
    public class ProjetoWebContext : DbContext
    {
        public ProjetoWebContext(DbContextOptions<ProjetoWebContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Department> Department { get; set; }
        public DbSet<Models.Seller> Seller { get; set; }
        public DbSet<Models.SalesRecord> SalesRecord { get; set; }
    }
}