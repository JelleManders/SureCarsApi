using Microsoft.EntityFrameworkCore;
using SureCarsApi.Models;

namespace SureCarsApi.DatabaseContext
{
    public class SureCarDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "SureCarDB");
        }

        public DbSet<SureCarDbo> SureCars { get; set; }
        public DbSet<SureUserDbo> SureUsers { get; set; }
    }
}
