using MachineManagement.Infrastructure.Data;
using MachineManagement.Core.Interfaces;
using MachineManagement.Infrastructure;
using MachineManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Configure Entity Framework with MySQL
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
            "Server=localhost;Database=hse_pm_db;Uid=root;Pwd=Anduongb67;Port=3306;";

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                b => b.MigrationsAssembly("MachineManagement.API")));

        // Register Unit of Work and Repositories
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
        builder.Services.AddScoped<ILineRepository, LineRepository>();
        builder.Services.AddScoped<IModelGroupRepository, ModelGroupRepository>();
        builder.Services.AddScoped<IModelRepository, ModelRepository>();
        builder.Services.AddScoped<IStationRepository, StationRepository>();
        builder.Services.AddScoped<IMachineTypeRepository, MachineTypeRepository>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure CORS for development
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("DevelopmentPolicy");
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // Health check endpoint
        app.MapGet("/api/health", () => "API is running!");

        app.Run();
    }
}
