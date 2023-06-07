using InsuranceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Services
{
    /// <summary>
    /// Represents the database context for the Insurance App.
    /// </summary>
    public class InsuranceAppDbContext : DbContext
    {
        public InsuranceAppDbContext(DbContextOptions<InsuranceAppDbContext> options) : base(options)
        {
        }

        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
