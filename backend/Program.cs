using Microsoft.EntityFrameworkCore;
using GarageLog.Models;
using GarageLog.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Setup SQLite with EF Core ---
builder.Services.AddDbContext<GarageLogContext>(options =>
    options.UseSqlite(@"Data Source=C:\Users\iowah\SourceCode\garage-log\backend\garage-log.db"));

// register services
builder.Services.AddScoped<ModService>();
// CORS for local frontend during development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();
// enable CORS for dev frontend
app.UseCors("AllowLocal");

// --- Create DB if it doesn't exist ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GarageLogContext>();
    db.Database.EnsureCreated();
    // Seed sample data if empty (handle existing DB without Cars table)
    bool needSeed = false;
    try
    {
        needSeed = !db.Cars.Any();
    }
    catch
    {
        // table may not exist; create it manually
        await db.Database.ExecuteSqlRawAsync(@"CREATE TABLE IF NOT EXISTS Cars (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Make TEXT,
            Model TEXT,
            Nickname TEXT,
            Year TEXT
        );");
        // If ModItems table is missing new columns, try adding them (ignore errors)
        try { await db.Database.ExecuteSqlRawAsync("ALTER TABLE ModItems ADD COLUMN CarId INTEGER;"); } catch { }
        try { await db.Database.ExecuteSqlRawAsync("ALTER TABLE ModItems ADD COLUMN PhotoPath TEXT;"); } catch { }
        try { await db.Database.ExecuteSqlRawAsync("ALTER TABLE ModItems ADD COLUMN ReceiptPath TEXT;"); } catch { }
        needSeed = true;
    }

    if (needSeed)
    {
        var civic = new Car { Make = "Honda", Model = "Civic", Nickname = "Civic", Year = "2006" };
        var van = new Car { Make = "Toyota", Model = "Sienna", Nickname = "Van", Year = "2012" };
        db.Cars.AddRange(civic, van);
        await db.SaveChangesAsync();

        db.ModItems.AddRange(
            new ModItem { Name = "Cold Air Intake", Category = "Intake", Cost = 299.99m, Status = "Complete", Notes = "Installed with heat shield and filter", Link = "", CarId = civic.Id, CreatedDate = DateTime.Now.AddMonths(-8), CompletedDate = DateTime.Now.AddMonths(-7), PhotoPath = "" },
            new ModItem { Name = "Coilover Kit", Category = "Suspension", Cost = 1200m, Status = "Planned", Notes = "Waiting for funds; researching brands", Link = "", CarId = civic.Id, CreatedDate = DateTime.Now.AddDays(-12), PhotoPath = "", ReceiptPath = "" },
            new ModItem { Name = "Exhaust", Category = "Exhaust", Cost = 450m, Status = "In Progress", Notes = "Welding muffler; shop appointment next week", Link = "", CarId = van.Id, CreatedDate = DateTime.Now.AddMonths(-2), PhotoPath = "" },

            // maintenance-like entries
            new ModItem { Name = "Oil Change", Category = "Maintenance", Cost = 39.99m, Status = "Complete", Notes = "Synthetic oil, filter replaced", Link = "", CarId = civic.Id, CreatedDate = DateTime.Now.AddMonths(-1), CompletedDate = DateTime.Now.AddMonths(-1), ReceiptPath = "" },
            new ModItem { Name = "Brake Pads (Rear)", Category = "Maintenance", Cost = 180m, Status = "Complete", Notes = "OEM pads installed", Link = "", CarId = van.Id, CreatedDate = DateTime.Now.AddMonths(-4), CompletedDate = DateTime.Now.AddMonths(-3), ReceiptPath = "" },

            // variety of mods
            new ModItem { Name = "Turbo Install", Category = "Engine", Cost = 3500m, Status = "Planned", Notes = "Major project; timeline TBD", Link = "", CarId = civic.Id, CreatedDate = DateTime.Now.AddMonths(-2) },
            new ModItem { Name = "Full Respray", Category = "Exterior", Cost = 4000m, Status = "Planned", Notes = "Color change to deep blue", Link = "", CarId = van.Id, CreatedDate = DateTime.Now.AddMonths(-6) },
            new ModItem { Name = "18\" Wheels", Category = "Wheels", Cost = 1200m, Status = "In Progress", Notes = "Staggered fitment arriving", Link = "", CarId = civic.Id, CreatedDate = DateTime.Now.AddDays(-20) },
            new ModItem { Name = "Head Unit + Speakers", Category = "Audio", Cost = 600m, Status = "Complete", Notes = "Bluetooth unit installed with amp", Link = "", CarId = van.Id, CreatedDate = DateTime.Now.AddMonths(-9), CompletedDate = DateTime.Now.AddMonths(-9) },
            new ModItem { Name = "Four-Wheel Alignment", Category = "Maintenance", Cost = 120m, Status = "Planned", Notes = "Schedule after wheel install", Link = "", CarId = civic.Id, CreatedDate = DateTime.Now.AddDays(-3) }
        );
        await db.SaveChangesAsync();
    }
}

app.MapGet("/mods", async (ModService service) =>
{
    var mods = await service.GetAllModsAsync();
    return Results.Ok(mods);
});

app.MapPost("/mods", async (ModItem mod, ModService service) =>
{
    var addedMod = await service.AddModAsync(mod);
    return Results.Created($"/mods/{addedMod.Id}", addedMod);
});

app.MapPatch("/mods/{id}", async (int id, ModItem updatedMod, ModService service) =>
{
    var mod = await service.UpdateModAsync(id, updatedMod);
    return mod != null ? Results.Ok(mod) : Results.NotFound();
});

app.MapDelete("/mods/{id}", async (int id, ModService service) =>
{
    var deleted = await service.DeleteModAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.MapGet("/mods/{id}", async (int id, ModService service) =>
{
    var mod = await service.GetModByIdAsync(id);
    return mod != null ? Results.Ok(mod) : Results.NotFound();
});

app.Run();

// --- DbContext ---
public class GarageLogContext : DbContext
{
    public GarageLogContext(DbContextOptions<GarageLogContext> options) : base(options) { }

    public DbSet<ModItem> ModItems { get; set; }
    public DbSet<Car> Cars { get; set; }
}
