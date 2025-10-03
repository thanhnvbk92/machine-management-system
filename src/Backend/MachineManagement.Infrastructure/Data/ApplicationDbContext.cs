using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;

namespace MachineManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        // DbSets for all entities
        public DbSet<Line> Lines { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<LogData> LogData { get; set; }
        public DbSet<Command> Commands { get; set; }
        public DbSet<ClientConfig> ClientConfigs { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<ModelGroup> ModelGroups { get; set; }
        public DbSet<ModelProcess> ModelProcesses { get; set; }
        public DbSet<MachineType> MachineTypes { get; set; }
        public DbSet<Model> Models { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships
            
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
                
            // Station -> ModelProcess relationship
            modelBuilder.Entity<Station>()
                .HasOne(s => s.ModelProcess)
                .WithMany(mp => mp.Stations)
                .HasForeignKey(s => s.ModelProcessId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // ModelProcess -> ModelGroup relationship
            modelBuilder.Entity<ModelProcess>()
                .HasOne(mp => mp.ModelGroup)
                .WithMany(mg => mg.ModelProcesses)
                .HasForeignKey(mp => mp.ModelGroupId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // ModelGroup -> Buyer relationship
            modelBuilder.Entity<ModelGroup>()
                .HasOne(mg => mg.Buyer)
                .WithMany(b => b.ModelGroups)
                .HasForeignKey(mg => mg.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Model -> ModelGroup relationship
            modelBuilder.Entity<Model>()
                .HasOne(m => m.ModelGroup)
                .WithMany(mg => mg.Models)
                .HasForeignKey(m => m.ModelGroupId)
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
        }
    }
}