using TVServiceCRM.Server.DataAccess.Configurations;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.DataAccess
{
    public class ApiDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
    {
        public ApiDbContext() { }
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ContactInformation> ContactInformations { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            ///Enable for ef update.
            //optionsBuilder.UseSqlServer("Server=localhost, 1433;Initial Catalog=TVServiceDB;Persist Security Info=False;User ID=sa;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");

            //optionsBuilder.UseSqlServer("Server=host.docker.internal, 1433;Initial Catalog=TVServiceDB;Persist Security Info=False;User ID=sa;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");


            //optionsBuilder.UseSqlite("Data Source=TVServiceCRM.db");
            optionsBuilder.UseSqlite("Data Source=" + Directory.GetCurrentDirectory() + "/VolumeDB/TVServiceCRM.db");

            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new TicketConfiguration());
            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new ContactInformationConfiguration());
        }
    }
}
