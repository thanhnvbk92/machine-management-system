using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MachineManagement.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            // Use a default connection string for migrations (doesn't need real MySQL)
            var connectionString = "Server=localhost;Database=machine_management_db;Uid=root;Pwd=password;Port=3306;CharSet=utf8mb4;";
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 25)));
            
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}