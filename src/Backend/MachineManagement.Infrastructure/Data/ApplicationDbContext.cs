using Microsoft.EntityFrameworkCore;
using MachineManagement.Core.Entities;

namespace MachineManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Machine> Machines { get; set; }
        public DbSet<LogData> LogData { get; set; }
        public DbSet<Command> Commands { get; set; }
        public DbSet<ClientConfig> ClientConfigs { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<ModelProcess> ModelProcesses { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<ModelGroup> ModelGroups { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships
            modelBuilder.Entity<Machine>()
                .HasOne(m => m.Station)
                .WithMany(s => s.Machines)
                .HasForeignKey(m => m.StationId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Station>()
                .HasOne(s => s.Line)
                .WithMany(l => l.Stations)
                .HasForeignKey(s => s.LineId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Line>()
                .HasOne(l => l.ModelProcess)
                .WithMany(mp => mp.Lines)
                .HasForeignKey(l => l.ModelProcessId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<ModelProcess>()
                .HasOne(mp => mp.Model)
                .WithMany(m => m.ModelProcesses)
                .HasForeignKey(mp => mp.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Model>()
                .HasOne(m => m.ModelGroup)
                .WithMany(mg => mg.Models)
                .HasForeignKey(m => m.ModelGroupId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<ModelGroup>()
                .HasOne(mg => mg.Buyer)
                .WithMany(b => b.ModelGroups)
                .HasForeignKey(mg => mg.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<LogData>()
                .HasOne(l => l.Machine)
                .WithMany(m => m.LogData)
                .HasForeignKey(l => l.MachineId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<Command>()
                .HasOne(c => c.Machine)
                .WithMany(m => m.Commands)
                .HasForeignKey(c => c.MachineId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<ClientConfig>()
                .HasOne(cc => cc.Machine)
                .WithMany()
                .HasForeignKey(cc => cc.MachineId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure indexes
            modelBuilder.Entity<Machine>()
                .HasIndex(m => m.MachineCode)
                .IsUnique();
                
            modelBuilder.Entity<LogData>()
                .HasIndex(l => new { l.MachineId, l.LogTimestamp });
                
            modelBuilder.Entity<Command>()
                .HasIndex(c => new { c.MachineId, c.Status });
                
            // Seed data
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Buyers
            modelBuilder.Entity<Buyer>().HasData(
                new Buyer { BuyerId = 1, BuyerName = "BMW", BuyerCode = "BMW", Description = "BMW Group" },
                new Buyer { BuyerId = 2, BuyerName = "Audi", BuyerCode = "AUDI", Description = "Audi AG" },
                new Buyer { BuyerId = 3, BuyerName = "Volkswagen", BuyerCode = "VW", Description = "Volkswagen Group" },
                new Buyer { BuyerId = 4, BuyerName = "Mercedes-Benz", BuyerCode = "MB", Description = "Mercedes-Benz Group" }
            );
            
            // Seed ModelGroups
            modelBuilder.Entity<ModelGroup>().HasData(
                new ModelGroup { ModelGroupId = 1, GroupName = "3 Series", GroupCode = "BMW_3", BuyerId = 1 },
                new ModelGroup { ModelGroupId = 2, GroupName = "A4", GroupCode = "AUDI_A4", BuyerId = 2 }
            );
            
            // Seed Models
            modelBuilder.Entity<Model>().HasData(
                new Model { ModelId = 1, ModelName = "BMW 320i", ModelCode = "BMW_320I", ModelGroupId = 1 },
                new Model { ModelId = 2, ModelName = "Audi A4 2.0T", ModelCode = "A4_20T", ModelGroupId = 2 }
            );
            
            // Seed ModelProcesses
            modelBuilder.Entity<ModelProcess>().HasData(
                new ModelProcess { ModelProcessId = 1, ProcessName = "Assembly", ProcessCode = "ASM", ModelId = 1 },
                new ModelProcess { ModelProcessId = 2, ProcessName = "Paint", ProcessCode = "PNT", ModelId = 1 }
            );
            
            // Seed Lines
            modelBuilder.Entity<Line>().HasData(
                new Line { LineId = 1, LineName = "Assembly Line 1", LineCode = "ASM_L1", ModelProcessId = 1 },
                new Line { LineId = 2, LineName = "Paint Line 1", LineCode = "PNT_L1", ModelProcessId = 2 }
            );
            
            // Seed Stations
            modelBuilder.Entity<Station>().HasData(
                new Station { StationId = 1, StationName = "Welding Station", StationCode = "WLD_S1", LineId = 1 },
                new Station { StationId = 2, StationName = "Paint Booth", StationCode = "PNT_B1", LineId = 2 }
            );
            
            // Seed Machines
            modelBuilder.Entity<Machine>().HasData(
                new Machine { MachineId = 1, MachineName = "Welding Robot 1", MachineCode = "WLD_R001", MachineType = "Robot", StationId = 1, Description = "Primary welding robot" },
                new Machine { MachineId = 2, MachineName = "Paint Sprayer 1", MachineCode = "PNT_S001", MachineType = "Sprayer", StationId = 2, Description = "Automated paint sprayer" }
            );
        }
    }
}