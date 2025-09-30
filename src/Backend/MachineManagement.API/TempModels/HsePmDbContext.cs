using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MachineManagement.API.TempModels;

public partial class HsePmDbContext : DbContext
{
    public HsePmDbContext(DbContextOptions<HsePmDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Buyer> Buyers { get; set; }

    public virtual DbSet<ClientConfig> ClientConfigs { get; set; }

    public virtual DbSet<Command> Commands { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Line> Lines { get; set; }

    public virtual DbSet<LogDatum> LogData { get; set; }

    public virtual DbSet<LogFile> LogFiles { get; set; }

    public virtual DbSet<Machine> Machines { get; set; }

    public virtual DbSet<Machinetype> Machinetypes { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Modelgroup> Modelgroups { get; set; }

    public virtual DbSet<Modelprocess> Modelprocesses { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<VMachineStatus> VMachineStatuses { get; set; }

    public virtual DbSet<VPendingCommand> VPendingCommands { get; set; }

    public virtual DbSet<VRecentLog> VRecentLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Buyer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("buyers");

            entity.HasIndex(e => e.Code, "Code").IsUnique();

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ClientConfig>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PRIMARY");

            entity.ToTable("client_config");

            entity.HasIndex(e => e.ConfigKey, "idx_config_key");

            entity.HasIndex(e => new { e.MachineId, e.ConfigKey }, "unique_machine_key").IsUnique();

            entity.Property(e => e.ConfigId).HasColumnName("config_id");
            entity.Property(e => e.ConfigKey)
                .HasMaxLength(100)
                .HasColumnName("config_key");
            entity.Property(e => e.ConfigValue)
                .HasColumnType("text")
                .HasColumnName("config_value");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.UpdatedTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_time");

            entity.HasOne(d => d.Machine).WithMany(p => p.ClientConfigs)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("client_config_machine_fk");
        });

        modelBuilder.Entity<Command>(entity =>
        {
            entity.HasKey(e => e.CommandId).HasName("PRIMARY");

            entity.ToTable("commands");

            entity.HasIndex(e => e.StationId, "commands_station_fk");

            entity.HasIndex(e => new { e.MachineId, e.Status }, "idx_machine_status");

            entity.HasIndex(e => new { e.Status, e.CreatedTime }, "idx_pending");

            entity.Property(e => e.CommandId).HasColumnName("command_id");
            entity.Property(e => e.CommandType)
                .HasMaxLength(50)
                .HasColumnName("command_type");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.ExecutedTime)
                .HasColumnType("datetime")
                .HasColumnName("executed_time");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Parameters)
                .HasColumnType("text")
                .HasColumnName("parameters");
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("'5'")
                .HasColumnName("priority");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(100)
                .HasColumnName("program_name");
            entity.Property(e => e.ResultMessage)
                .HasColumnType("text")
                .HasColumnName("result_message");
            entity.Property(e => e.SentTime)
                .HasColumnType("datetime")
                .HasColumnName("sent_time");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'")
                .HasColumnName("status");

            entity.HasOne(d => d.Machine).WithMany(p => p.Commands)
                .HasForeignKey(d => d.MachineId)
                .HasConstraintName("commands_machine_fk");

            entity.HasOne(d => d.Station).WithMany(p => p.Commands)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("commands_station_fk");
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Line>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lines");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<LogDatum>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PRIMARY");

            entity.ToTable("log_data");

            entity.HasIndex(e => new { e.LogLevel, e.ReceivedTime }, "idx_log_level");

            entity.HasIndex(e => new { e.MachineId, e.StartTime }, "idx_machine_time");

            entity.HasIndex(e => new { e.ModelId, e.StartTime }, "idx_model_time");

            entity.HasIndex(e => new { e.Result, e.ReceivedTime }, "idx_result");

            entity.HasIndex(e => e.FileId, "log_data_file_fk");

            entity.HasIndex(e => e.StationId, "log_data_station_fk");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.Eqpinfo)
                .HasMaxLength(50)
                .HasColumnName("EQPINFO");
            entity.Property(e => e.Fid)
                .HasMaxLength(22)
                .HasColumnName("FID");
            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.GmesStatus)
                .HasMaxLength(10)
                .HasColumnName("gmes_status");
            entity.Property(e => e.Jobfile)
                .HasMaxLength(100)
                .HasColumnName("jobfile");
            entity.Property(e => e.LogLevel)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Info'")
                .HasColumnName("log_level");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Measure)
                .HasMaxLength(255)
                .HasColumnName("measure");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.PartNo)
                .HasMaxLength(11)
                .HasColumnName("part_no");
            entity.Property(e => e.Pid)
                .HasMaxLength(22)
                .HasColumnName("PID");
            entity.Property(e => e.Procinfo)
                .HasMaxLength(10)
                .HasColumnName("PROCINFO");
            entity.Property(e => e.RawData)
                .HasColumnType("text")
                .HasColumnName("raw_data");
            entity.Property(e => e.ReceivedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("received_time");
            entity.Property(e => e.Result)
                .HasMaxLength(15)
                .HasColumnName("result");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .HasColumnName("source");
            entity.Property(e => e.SpecMax)
                .HasMaxLength(255)
                .HasColumnName("spec_max");
            entity.Property(e => e.SpecMin)
                .HasMaxLength(255)
                .HasColumnName("spec_min");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.StepNg)
                .HasMaxLength(255)
                .HasColumnName("stepNG");
            entity.Property(e => e.Variant)
                .HasMaxLength(20)
                .HasColumnName("variant");

            entity.HasOne(d => d.File).WithMany(p => p.LogData)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("log_data_file_fk");

            entity.HasOne(d => d.Machine).WithMany(p => p.LogData)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("log_data_machine_fk");

            entity.HasOne(d => d.Model).WithMany(p => p.LogData)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("log_data_model_fk");

            entity.HasOne(d => d.Station).WithMany(p => p.LogData)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("log_data_station_fk");
        });

        modelBuilder.Entity<LogFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("log_file");

            entity.HasIndex(e => e.FileName, "idx_file_name");

            entity.HasIndex(e => new { e.MachineId, e.DateCreated }, "idx_machine_date");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.FileName)
                .HasMaxLength(100)
                .HasColumnName("file_name");
            entity.Property(e => e.FileSize)
                .HasDefaultValueSql("'0'")
                .HasColumnName("file_size");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Processing'")
                .HasColumnName("status");

            entity.HasOne(d => d.Machine).WithMany(p => p.LogFiles)
                .HasForeignKey(d => d.MachineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("log_file_machine_fk");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("machines");

            entity.HasIndex(e => e.Ip, "IP_UNIQUE").IsUnique();

            entity.HasIndex(e => e.StationId, "StationID");

            entity.HasIndex(e => new { e.ClientStatus, e.LastSeen }, "idx_client_status");

            entity.HasIndex(e => e.MacAddress, "idx_machines_mac").IsUnique();

            entity.HasIndex(e => e.MachineTypeId, "machines_machinetypes_idx");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AppVersion)
                .HasMaxLength(50)
                .HasColumnName("app_version");
            entity.Property(e => e.ClientStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Offline'")
                .HasColumnName("client_status");
            entity.Property(e => e.GmesName)
                .HasMaxLength(50)
                .HasColumnName("GMES_Name");
            entity.Property(e => e.Ip)
                .HasMaxLength(15)
                .HasColumnName("IP");
            entity.Property(e => e.LastLogTime)
                .HasColumnType("datetime")
                .HasColumnName("last_log_time");
            entity.Property(e => e.LastSeen)
                .HasColumnType("datetime")
                .HasColumnName("last_seen");
            entity.Property(e => e.MacAddress)
                .HasMaxLength(50)
                .HasComment("MAC address of the machine")
                .HasColumnName("mac_address");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ProgramName).HasMaxLength(45);
            entity.Property(e => e.StationId).HasColumnName("StationID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.MachineType).WithMany(p => p.Machines)
                .HasForeignKey(d => d.MachineTypeId)
                .HasConstraintName("machines_machinetypes");

            entity.HasOne(d => d.Station).WithMany(p => p.Machines)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("machines_ibfk_1");
        });

        modelBuilder.Entity<Machinetype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("machinetypes");

            entity.Property(e => e.Name).HasMaxLength(45);
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("models");

            entity.HasIndex(e => e.ModelGroupId, "ModelGroupID");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ModelGroupId).HasColumnName("ModelGroupID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.ModelGroup).WithMany(p => p.Models)
                .HasForeignKey(d => d.ModelGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("models_ibfk_1");
        });

        modelBuilder.Entity<Modelgroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("modelgroups");

            entity.HasIndex(e => e.BuyerId, "BuyerId");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Buyer).WithMany(p => p.Modelgroups)
                .HasForeignKey(d => d.BuyerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modelgroups_ibfk_1");
        });

        modelBuilder.Entity<Modelprocess>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("modelprocesses");

            entity.HasIndex(e => e.ModelGroupId, "ModelGroupID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ModelGroupId).HasColumnName("ModelGroupID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.ModelGroup).WithMany(p => p.Modelprocesses)
                .HasForeignKey(d => d.ModelGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modelprocesses_ibfk_1");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("stations");

            entity.HasIndex(e => e.LineId, "LineId");

            entity.HasIndex(e => e.ModelProcessId, "stations_ibfk_1");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Line).WithMany(p => p.Stations)
                .HasForeignKey(d => d.LineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stations_ibfk_2");

            entity.HasOne(d => d.ModelProcess).WithMany(p => p.Stations)
                .HasForeignKey(d => d.ModelProcessId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stations_ibfk_1");
        });

        modelBuilder.Entity<VMachineStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_machine_status");

            entity.Property(e => e.AppVersion)
                .HasMaxLength(50)
                .HasColumnName("app_version");
            entity.Property(e => e.ClientStatus)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Offline'")
                .HasColumnName("client_status");
            entity.Property(e => e.GmesName)
                .HasMaxLength(50)
                .HasColumnName("GMES_Name");
            entity.Property(e => e.Ip)
                .HasMaxLength(15)
                .HasColumnName("IP");
            entity.Property(e => e.LastSeen)
                .HasColumnType("datetime")
                .HasColumnName("last_seen");
            entity.Property(e => e.LineName)
                .HasMaxLength(20)
                .HasColumnName("line_name");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.MachineName)
                .HasMaxLength(100)
                .HasColumnName("machine_name");
            entity.Property(e => e.MachineStatus)
                .HasMaxLength(20)
                .HasColumnName("machine_status");
            entity.Property(e => e.ProgramName).HasMaxLength(45);
            entity.Property(e => e.StationName)
                .HasMaxLength(50)
                .HasColumnName("station_name");
        });

        modelBuilder.Entity<VPendingCommand>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_pending_commands");

            entity.Property(e => e.CommandId).HasColumnName("command_id");
            entity.Property(e => e.CommandType)
                .HasMaxLength(50)
                .HasColumnName("command_type");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.MachineName)
                .HasMaxLength(100)
                .HasColumnName("machine_name");
            entity.Property(e => e.Priority)
                .HasDefaultValueSql("'5'")
                .HasColumnName("priority");
            entity.Property(e => e.ProgramName)
                .HasMaxLength(100)
                .HasColumnName("program_name");
            entity.Property(e => e.StationName)
                .HasMaxLength(50)
                .HasColumnName("station_name");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'")
                .HasColumnName("status");
        });

        modelBuilder.Entity<VRecentLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_recent_logs");

            entity.Property(e => e.LineName)
                .HasMaxLength(20)
                .HasColumnName("line_name");
            entity.Property(e => e.LogLevel)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Info'")
                .HasColumnName("log_level");
            entity.Property(e => e.MachineName)
                .HasMaxLength(100)
                .HasColumnName("machine_name");
            entity.Property(e => e.ModelName)
                .HasMaxLength(50)
                .HasColumnName("model_name");
            entity.Property(e => e.Pid)
                .HasMaxLength(22)
                .HasColumnName("PID");
            entity.Property(e => e.Result)
                .HasMaxLength(15)
                .HasColumnName("result");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .HasColumnName("source");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.StationName)
                .HasMaxLength(50)
                .HasColumnName("station_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
