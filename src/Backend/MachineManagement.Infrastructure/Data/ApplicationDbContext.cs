using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;

namespace MachineManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        // DbSets for all entities
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<MachineType> MachineTypes { get; set; }
        public DbSet<ModelGroup> ModelGroups { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<ModelProcess> ModelProcesses { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<LogFile> LogFiles { get; set; }
        public DbSet<LogData> LogData { get; set; }
        public DbSet<Command> Commands { get; set; }
        public DbSet<ClientConfig> ClientConfigs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure basic relationships that exist
            
            // Machine -> Station relationship
            modelBuilder.Entity<Machine>()
                .HasOne(m => m.Station)
                .WithMany(s => s.Machines)
                .HasForeignKey(m => m.StationId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Machine -> MachineType relationship
            modelBuilder.Entity<Machine>()
                .HasOne(m => m.MachineType)
                .WithMany(mt => mt.Machines)
                .HasForeignKey(m => m.MachineTypeId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Station -> Line relationship
            modelBuilder.Entity<Station>()
                .HasOne(s => s.Line)
                .WithMany(l => l.Stations)
                .HasForeignKey(s => s.LineId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Buyer -> ModelGroup relationship (cập nhật theo database thực tế)
            modelBuilder.Entity<ModelGroup>()
                .HasOne(mg => mg.Buyer)
                .WithMany(b => b.ModelGroups)
                .HasForeignKey(mg => mg.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // ClientConfig primary key configuration
            modelBuilder.Entity<ClientConfig>()
                .HasKey(cc => cc.ConfigId);
                
            // Command primary key configuration
            modelBuilder.Entity<Command>()
                .HasKey(c => c.CommandId);
                
            // LogData primary key configuration
            modelBuilder.Entity<LogData>()
                .HasKey(ld => ld.LogId);
                
            // Machine configuration to ignore BaseEntity properties that don't exist in database
            modelBuilder.Entity<Machine>()
                .Ignore(m => m.CreatedAt)
                .Ignore(m => m.UpdatedAt)
                .Ignore(m => m.IsActive);
                
            // Remove index cho SerialNumber vì không còn field này trong Machine entity
        }
    }
}