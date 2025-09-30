using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MachineManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAppVersionToMachines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CLIENT_CONFIGS_machines_MachineId",
                table: "CLIENT_CONFIGS");

            migrationBuilder.DropForeignKey(
                name: "FK_COMMANDS_machines_MachineId",
                table: "COMMANDS");

            migrationBuilder.DropForeignKey(
                name: "FK_lines_buyers_BuyerId",
                table: "lines");

            migrationBuilder.DropForeignKey(
                name: "FK_LOGDATA_log_file_LogFileId",
                table: "LOGDATA");

            migrationBuilder.DropForeignKey(
                name: "FK_LOGDATA_machines_MachineId",
                table: "LOGDATA");

            migrationBuilder.DropForeignKey(
                name: "FK_LOGDATA_models_ModelId",
                table: "LOGDATA");

            migrationBuilder.DropIndex(
                name: "IX_machines_SerialNumber",
                table: "machines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_COMMANDS",
                table: "COMMANDS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LOGDATA",
                table: "LOGDATA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CLIENT_CONFIGS",
                table: "CLIENT_CONFIGS");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "stations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "modelgroups");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "LastMaintenance",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "CommandData",
                table: "COMMANDS");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "COMMANDS");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "buyers");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "LOGDATA");

            migrationBuilder.DropColumn(
                name: "LogType",
                table: "LOGDATA");

            migrationBuilder.DropColumn(
                name: "DataType",
                table: "CLIENT_CONFIGS");

            migrationBuilder.DropColumn(
                name: "IsEncrypted",
                table: "CLIENT_CONFIGS");

            migrationBuilder.RenameTable(
                name: "COMMANDS",
                newName: "commands");

            migrationBuilder.RenameTable(
                name: "LOGDATA",
                newName: "log_data");

            migrationBuilder.RenameTable(
                name: "CLIENT_CONFIGS",
                newName: "client_config");

            migrationBuilder.RenameColumn(
                name: "StationOrder",
                table: "stations",
                newName: "ModelProcessId");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "machines",
                newName: "LastSeen");

            migrationBuilder.RenameColumn(
                name: "NextMaintenance",
                table: "machines",
                newName: "LastLogTime");

            migrationBuilder.RenameColumn(
                name: "ScheduledAt",
                table: "commands",
                newName: "SentTime");

            migrationBuilder.RenameColumn(
                name: "Response",
                table: "commands",
                newName: "ResultMessage");

            migrationBuilder.RenameColumn(
                name: "ExecutedAt",
                table: "commands",
                newName: "ExecutedTime");

            migrationBuilder.RenameIndex(
                name: "IX_COMMANDS_MachineId",
                table: "commands",
                newName: "IX_commands_MachineId");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "log_data",
                newName: "Result");

            migrationBuilder.RenameColumn(
                name: "LogTimestamp",
                table: "log_data",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "LogFileId",
                table: "log_data",
                newName: "StationId");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "log_data",
                newName: "Variant");

            migrationBuilder.RenameIndex(
                name: "IX_LOGDATA_ModelId",
                table: "log_data",
                newName: "IX_log_data_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_LOGDATA_MachineId",
                table: "log_data",
                newName: "IX_log_data_MachineId");

            migrationBuilder.RenameIndex(
                name: "IX_LOGDATA_LogFileId",
                table: "log_data",
                newName: "IX_log_data_StationId");

            migrationBuilder.RenameIndex(
                name: "IX_CLIENT_CONFIGS_MachineId",
                table: "client_config",
                newName: "IX_client_config_MachineId");

            migrationBuilder.AddColumn<int>(
                name: "ModelGroupId",
                table: "modelprocesses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BuyerId",
                table: "modelgroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "machines",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "StationId",
                table: "machines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineTypeId",
                table: "machines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AppVersion",
                table: "machines",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ClientStatus",
                table: "machines",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GmesName",
                table: "machines",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "machines",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MacAddress",
                table: "machines",
                type: "varchar(17)",
                maxLength: 17,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProgramName",
                table: "machines",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "MachineId",
                table: "log_file",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "commands",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "commands",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "commands",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CommandType",
                table: "commands",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "CommandId",
                table: "commands",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "commands",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "commands",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parameters",
                table: "commands",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProgramName",
                table: "commands",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "StationId",
                table: "commands",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "buyers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "log_data",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "LogLevel",
                table: "log_data",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "LogId",
                table: "log_data",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "log_data",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "log_data",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Eqpinfo",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Fid",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "log_data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GmesStatus",
                table: "log_data",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Jobfile",
                table: "log_data",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Measure",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PartNo",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Pid",
                table: "log_data",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Procinfo",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RawData",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedTime",
                table: "log_data",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecMax",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SpecMin",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StepNg",
                table: "log_data",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "client_config",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "client_config",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConfigValue",
                table: "client_config",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConfigKey",
                table: "client_config",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ConfigId",
                table: "client_config",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "client_config",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "client_config",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_commands",
                table: "commands",
                column: "CommandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_log_data",
                table: "log_data",
                column: "LogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_client_config",
                table: "client_config",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_stations_ModelProcessId",
                table: "stations",
                column: "ModelProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_modelprocesses_ModelGroupId",
                table: "modelprocesses",
                column: "ModelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_modelgroups_BuyerId",
                table: "modelgroups",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_log_file_MachineId",
                table: "log_file",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_commands_StationId",
                table: "commands",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_log_data_FileId",
                table: "log_data",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_client_config_machines_MachineId",
                table: "client_config",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_commands_machines_MachineId",
                table: "commands",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_commands_stations_StationId",
                table: "commands",
                column: "StationId",
                principalTable: "stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lines_buyers_BuyerId",
                table: "lines",
                column: "BuyerId",
                principalTable: "buyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_log_data_log_file_FileId",
                table: "log_data",
                column: "FileId",
                principalTable: "log_file",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_log_data_machines_MachineId",
                table: "log_data",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_log_data_models_ModelId",
                table: "log_data",
                column: "ModelId",
                principalTable: "models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_log_data_stations_StationId",
                table: "log_data",
                column: "StationId",
                principalTable: "stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_log_file_machines_MachineId",
                table: "log_file",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_modelgroups_buyers_BuyerId",
                table: "modelgroups",
                column: "BuyerId",
                principalTable: "buyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_modelprocesses_modelgroups_ModelGroupId",
                table: "modelprocesses",
                column: "ModelGroupId",
                principalTable: "modelgroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_stations_modelprocesses_ModelProcessId",
                table: "stations",
                column: "ModelProcessId",
                principalTable: "modelprocesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_client_config_machines_MachineId",
                table: "client_config");

            migrationBuilder.DropForeignKey(
                name: "FK_commands_machines_MachineId",
                table: "commands");

            migrationBuilder.DropForeignKey(
                name: "FK_commands_stations_StationId",
                table: "commands");

            migrationBuilder.DropForeignKey(
                name: "FK_lines_buyers_BuyerId",
                table: "lines");

            migrationBuilder.DropForeignKey(
                name: "FK_log_data_log_file_FileId",
                table: "log_data");

            migrationBuilder.DropForeignKey(
                name: "FK_log_data_machines_MachineId",
                table: "log_data");

            migrationBuilder.DropForeignKey(
                name: "FK_log_data_models_ModelId",
                table: "log_data");

            migrationBuilder.DropForeignKey(
                name: "FK_log_data_stations_StationId",
                table: "log_data");

            migrationBuilder.DropForeignKey(
                name: "FK_log_file_machines_MachineId",
                table: "log_file");

            migrationBuilder.DropForeignKey(
                name: "FK_modelgroups_buyers_BuyerId",
                table: "modelgroups");

            migrationBuilder.DropForeignKey(
                name: "FK_modelprocesses_modelgroups_ModelGroupId",
                table: "modelprocesses");

            migrationBuilder.DropForeignKey(
                name: "FK_stations_modelprocesses_ModelProcessId",
                table: "stations");

            migrationBuilder.DropIndex(
                name: "IX_stations_ModelProcessId",
                table: "stations");

            migrationBuilder.DropIndex(
                name: "IX_modelprocesses_ModelGroupId",
                table: "modelprocesses");

            migrationBuilder.DropIndex(
                name: "IX_modelgroups_BuyerId",
                table: "modelgroups");

            migrationBuilder.DropIndex(
                name: "IX_log_file_MachineId",
                table: "log_file");

            migrationBuilder.DropPrimaryKey(
                name: "PK_commands",
                table: "commands");

            migrationBuilder.DropIndex(
                name: "IX_commands_StationId",
                table: "commands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_log_data",
                table: "log_data");

            migrationBuilder.DropIndex(
                name: "IX_log_data_FileId",
                table: "log_data");

            migrationBuilder.DropPrimaryKey(
                name: "PK_client_config",
                table: "client_config");

            migrationBuilder.DropColumn(
                name: "ModelGroupId",
                table: "modelprocesses");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "modelgroups");

            migrationBuilder.DropColumn(
                name: "AppVersion",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "ClientStatus",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "GmesName",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "Ip",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "MacAddress",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "ProgramName",
                table: "machines");

            migrationBuilder.DropColumn(
                name: "MachineId",
                table: "log_file");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "commands");

            migrationBuilder.DropColumn(
                name: "Parameters",
                table: "commands");

            migrationBuilder.DropColumn(
                name: "ProgramName",
                table: "commands");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "commands");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "buyers");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "Eqpinfo",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "Fid",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "GmesStatus",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "Jobfile",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "Measure",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "PartNo",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "Pid",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "Procinfo",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "RawData",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "ReceivedTime",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "SpecMax",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "SpecMin",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "StepNg",
                table: "log_data");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "client_config");

            migrationBuilder.RenameTable(
                name: "commands",
                newName: "COMMANDS");

            migrationBuilder.RenameTable(
                name: "log_data",
                newName: "LOGDATA");

            migrationBuilder.RenameTable(
                name: "client_config",
                newName: "CLIENT_CONFIGS");

            migrationBuilder.RenameColumn(
                name: "ModelProcessId",
                table: "stations",
                newName: "StationOrder");

            migrationBuilder.RenameColumn(
                name: "LastSeen",
                table: "machines",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "LastLogTime",
                table: "machines",
                newName: "NextMaintenance");

            migrationBuilder.RenameColumn(
                name: "SentTime",
                table: "COMMANDS",
                newName: "ScheduledAt");

            migrationBuilder.RenameColumn(
                name: "ResultMessage",
                table: "COMMANDS",
                newName: "Response");

            migrationBuilder.RenameColumn(
                name: "ExecutedTime",
                table: "COMMANDS",
                newName: "ExecutedAt");

            migrationBuilder.RenameIndex(
                name: "IX_commands_MachineId",
                table: "COMMANDS",
                newName: "IX_COMMANDS_MachineId");

            migrationBuilder.RenameColumn(
                name: "Variant",
                table: "LOGDATA",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "LOGDATA",
                newName: "LogFileId");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "LOGDATA",
                newName: "LogTimestamp");

            migrationBuilder.RenameColumn(
                name: "Result",
                table: "LOGDATA",
                newName: "Message");

            migrationBuilder.RenameIndex(
                name: "IX_log_data_StationId",
                table: "LOGDATA",
                newName: "IX_LOGDATA_LogFileId");

            migrationBuilder.RenameIndex(
                name: "IX_log_data_ModelId",
                table: "LOGDATA",
                newName: "IX_LOGDATA_ModelId");

            migrationBuilder.RenameIndex(
                name: "IX_log_data_MachineId",
                table: "LOGDATA",
                newName: "IX_LOGDATA_MachineId");

            migrationBuilder.RenameIndex(
                name: "IX_client_config_MachineId",
                table: "CLIENT_CONFIGS",
                newName: "IX_CLIENT_CONFIGS_MachineId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "stations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "modelgroups",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "machines",
                keyColumn: "Status",
                keyValue: null,
                column: "Status",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "machines",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "StationId",
                table: "machines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineTypeId",
                table: "machines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "machines",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "machines",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "machines",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMaintenance",
                table: "machines",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "machines",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "COMMANDS",
                keyColumn: "Status",
                keyValue: null,
                column: "Status",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "COMMANDS",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "COMMANDS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "COMMANDS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "COMMANDS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "CommandType",
                table: "COMMANDS",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "CommandId",
                table: "COMMANDS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "CommandData",
                table: "COMMANDS",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "COMMANDS",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "buyers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "LOGDATA",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "LOGDATA",
                keyColumn: "LogLevel",
                keyValue: null,
                column: "LogLevel",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "LogLevel",
                table: "LOGDATA",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "LOGDATA",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "LogId",
                table: "LOGDATA",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "LOGDATA",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LogType",
                table: "LOGDATA",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "MachineId",
                table: "CLIENT_CONFIGS",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CLIENT_CONFIGS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CLIENT_CONFIGS",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CLIENT_CONFIGS",
                keyColumn: "ConfigValue",
                keyValue: null,
                column: "ConfigValue",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConfigValue",
                table: "CLIENT_CONFIGS",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConfigKey",
                table: "CLIENT_CONFIGS",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ConfigId",
                table: "CLIENT_CONFIGS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "CLIENT_CONFIGS",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsEncrypted",
                table: "CLIENT_CONFIGS",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_COMMANDS",
                table: "COMMANDS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LOGDATA",
                table: "LOGDATA",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CLIENT_CONFIGS",
                table: "CLIENT_CONFIGS",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_machines_SerialNumber",
                table: "machines",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CLIENT_CONFIGS_machines_MachineId",
                table: "CLIENT_CONFIGS",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_COMMANDS_machines_MachineId",
                table: "COMMANDS",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lines_buyers_BuyerId",
                table: "lines",
                column: "BuyerId",
                principalTable: "buyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LOGDATA_log_file_LogFileId",
                table: "LOGDATA",
                column: "LogFileId",
                principalTable: "log_file",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LOGDATA_machines_MachineId",
                table: "LOGDATA",
                column: "MachineId",
                principalTable: "machines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LOGDATA_models_ModelId",
                table: "LOGDATA",
                column: "ModelId",
                principalTable: "models",
                principalColumn: "Id");
        }
    }
}
