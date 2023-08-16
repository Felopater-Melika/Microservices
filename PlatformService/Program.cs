using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;
var commandServiceUrl = configuration["commandService"];
Console.WriteLine($"--> Command Service URL: {commandServiceUrl}");
var connectionString = configuration.GetConnectionString("PlatformsConn");
Console.WriteLine($"--> Connection String: {connectionString}");

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (env.IsProduction())
{
    Console.WriteLine($"--> Using SQL Server DB");
    builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseSqlServer(configuration.GetConnectionString("PlatformsConn"))
    );
}
else
{
    Console.WriteLine($"--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

PrepDb.PrepPopulation(app, env.IsProduction());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
