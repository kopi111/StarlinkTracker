using Microsoft.EntityFrameworkCore;
using StarlinkTracker.Data;
using StarlinkTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Starlink Tracker API",
        Version = "v1",
        Description = "API for tracking Starlink device placements across Jamaica"
    });
});

// Add CORS for web clients
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure SQL Server database
builder.Services.AddDbContext<StarlinkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(60)));

// Register services
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<GeoJsonService>();

// Enable static files for map visualization
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Database already exists on remote server - no need to create
// Uncomment below only if you need to create database locally
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<StarlinkDbContext>();
//     db.Database.EnsureCreated();
// }

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Starlink Tracker API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map root to map visualization
app.MapGet("/", () => Results.Redirect("/map.html"));

app.Run();
