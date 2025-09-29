using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MachineManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BUYERS",
                columns: table => new
                {
                    BuyerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BuyerName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BuyerCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BUYERS", x => x.BuyerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MODELGROUPS",
                columns: table => new
                {
                    ModelGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GroupName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BuyerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODELGROUPS", x => x.ModelGroupId);
                    table.ForeignKey(
                        name: "FK_MODELGROUPS_BUYERS_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "BUYERS",
                        principalColumn: "BuyerId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MODELS",
                columns: table => new
                {
                    ModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ModelName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModelCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModelGroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODELS", x => x.ModelId);
                    table.ForeignKey(
                        name: "FK_MODELS_MODELGROUPS_ModelGroupId",
                        column: x => x.ModelGroupId,
                        principalTable: "MODELGROUPS",
                        principalColumn: "ModelGroupId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MODELPROCESSES",
                columns: table => new
                {
                    ModelProcessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProcessName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProcessCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODELPROCESSES", x => x.ModelProcessId);
                    table.ForeignKey(
                        name: "FK_MODELPROCESSES_MODELS_ModelId",
                        column: x => x.ModelId,
                        principalTable: "MODELS",
                        principalColumn: "ModelId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LINES",
                columns: table => new
                {
                    LineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LineName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LineCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModelProcessId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LINES", x => x.LineId);
                    table.ForeignKey(
                        name: "FK_LINES_MODELPROCESSES_ModelProcessId",
                        column: x => x.ModelProcessId,
                        principalTable: "MODELPROCESSES",
                        principalColumn: "ModelProcessId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "STATIONS",
                columns: table => new
                {
                    StationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StationName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StationCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LineId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STATIONS", x => x.StationId);
                    table.ForeignKey(
                        name: "FK_STATIONS_LINES_LineId",
                        column: x => x.LineId,
                        principalTable: "LINES",
                        principalColumn: "LineId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MACHINES",
                columns: table => new
                {
                    MachineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MachineName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MachineCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MachineType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MACHINES", x => x.MachineId);
                    table.ForeignKey(
                        name: "FK_MACHINES_STATIONS_StationId",
                        column: x => x.StationId,
                        principalTable: "STATIONS",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CLIENT_CONFIGS",
                columns: table => new
                {
                    ConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    ConfigKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConfigValue = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEncrypted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENT_CONFIGS", x => x.ConfigId);
                    table.ForeignKey(
                        name: "FK_CLIENT_CONFIGS_MACHINES_MachineId",
                        column: x => x.MachineId,
                        principalTable: "MACHINES",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "COMMANDS",
                columns: table => new
                {
                    CommandId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    CommandType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommandData = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExecutedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Response = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErrorMessage = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMMANDS", x => x.CommandId);
                    table.ForeignKey(
                        name: "FK_COMMANDS_MACHINES_MachineId",
                        column: x => x.MachineId,
                        principalTable: "MACHINES",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LOGDATA",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    LogType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogLevel = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogTimestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Source = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOGDATA", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_LOGDATA_MACHINES_MachineId",
                        column: x => x.MachineId,
                        principalTable: "MACHINES",
                        principalColumn: "MachineId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "BUYERS",
                columns: new[] { "BuyerId", "BuyerCode", "BuyerName", "CreatedAt", "Description", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "BMW", "BMW", new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(3798), "BMW Group", true, null },
                    { 2, "AUDI", "Audi", new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(3803), "Audi AG", true, null },
                    { 3, "VW", "Volkswagen", new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(3805), "Volkswagen Group", true, null },
                    { 4, "MB", "Mercedes-Benz", new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(3807), "Mercedes-Benz Group", true, null }
                });

            migrationBuilder.InsertData(
                table: "MODELGROUPS",
                columns: new[] { "ModelGroupId", "BuyerId", "CreatedAt", "Description", "GroupCode", "GroupName", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4028), null, "BMW_3", "3 Series", true, null },
                    { 2, 2, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4032), null, "AUDI_A4", "A4", true, null }
                });

            migrationBuilder.InsertData(
                table: "MODELS",
                columns: new[] { "ModelId", "CreatedAt", "Description", "IsActive", "ModelCode", "ModelGroupId", "ModelName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4091), null, true, "BMW_320I", 1, "BMW 320i", null },
                    { 2, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4111), null, true, "A4_20T", 2, "Audi A4 2.0T", null }
                });

            migrationBuilder.InsertData(
                table: "MODELPROCESSES",
                columns: new[] { "ModelProcessId", "CreatedAt", "Description", "IsActive", "ModelId", "ProcessCode", "ProcessName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4189), null, true, 1, "ASM", "Assembly", null },
                    { 2, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4200), null, true, 1, "PNT", "Paint", null }
                });

            migrationBuilder.InsertData(
                table: "LINES",
                columns: new[] { "LineId", "CreatedAt", "Description", "IsActive", "LineCode", "LineName", "ModelProcessId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4264), null, true, "ASM_L1", "Assembly Line 1", 1, null },
                    { 2, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4267), null, true, "PNT_L1", "Paint Line 1", 2, null }
                });

            migrationBuilder.InsertData(
                table: "STATIONS",
                columns: new[] { "StationId", "CreatedAt", "Description", "IsActive", "LineId", "StationCode", "StationName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4343), null, true, 1, "WLD_S1", "Welding Station", null },
                    { 2, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4359), null, true, 2, "PNT_B1", "Paint Booth", null }
                });

            migrationBuilder.InsertData(
                table: "MACHINES",
                columns: new[] { "MachineId", "CreatedAt", "Description", "IsActive", "MachineCode", "MachineName", "MachineType", "StationId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4413), "Primary welding robot", true, "WLD_R001", "Welding Robot 1", "Robot", 1, null },
                    { 2, new DateTime(2025, 9, 29, 0, 56, 55, 162, DateTimeKind.Utc).AddTicks(4418), "Automated paint sprayer", true, "PNT_S001", "Paint Sprayer 1", "Sprayer", 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLIENT_CONFIGS_MachineId",
                table: "CLIENT_CONFIGS",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_COMMANDS_MachineId_Status",
                table: "COMMANDS",
                columns: new[] { "MachineId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LINES_ModelProcessId",
                table: "LINES",
                column: "ModelProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_LOGDATA_MachineId_LogTimestamp",
                table: "LOGDATA",
                columns: new[] { "MachineId", "LogTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_MACHINES_MachineCode",
                table: "MACHINES",
                column: "MachineCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MACHINES_StationId",
                table: "MACHINES",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_MODELGROUPS_BuyerId",
                table: "MODELGROUPS",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_MODELPROCESSES_ModelId",
                table: "MODELPROCESSES",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MODELS_ModelGroupId",
                table: "MODELS",
                column: "ModelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_STATIONS_LineId",
                table: "STATIONS",
                column: "LineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CLIENT_CONFIGS");

            migrationBuilder.DropTable(
                name: "COMMANDS");

            migrationBuilder.DropTable(
                name: "LOGDATA");

            migrationBuilder.DropTable(
                name: "MACHINES");

            migrationBuilder.DropTable(
                name: "STATIONS");

            migrationBuilder.DropTable(
                name: "LINES");

            migrationBuilder.DropTable(
                name: "MODELPROCESSES");

            migrationBuilder.DropTable(
                name: "MODELS");

            migrationBuilder.DropTable(
                name: "MODELGROUPS");

            migrationBuilder.DropTable(
                name: "BUYERS");
        }
    }
}
