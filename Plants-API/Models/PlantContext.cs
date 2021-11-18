using Microsoft.EntityFrameworkCore;

namespace Plants_API.Models
{
    public class PlantContext : DbContext
    {
        public PlantContext(DbContextOptions<PlantContext> options)
               : base(options)
        {
        }

        public DbSet<Plant> plants { get; set; } = null!;
    }
}
