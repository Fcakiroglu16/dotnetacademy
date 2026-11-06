using Clinic.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
// Absolute paths, so `dotnet run` and `dotnet ef` (different working directories) hit the same files.
var dir = Path.Combine(Path.GetTempPath(), "clinic-lesson");
Directory.CreateDirectory(dir);
builder.Services.AddDbContext<ClinicDbContext>(o => o.UseSqlite($"Data Source={Path.Combine(dir, "clinic.db")}"));
builder.Services.AddDbContext<ScheduleDbContext>(o => o.UseSqlite($"Data Source={Path.Combine(dir, "schedule.db")}"));
using var host = builder.Build();

// dotnet ef stops at Build() above; everything below only runs with `dotnet run`.
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();

Console.WriteLine("1) vaccination record for a pet that is not registered yet (FK excluded):");
db.Vaccinations.Add(new VaccinationRecord { PetId = 404, Vaccine = "Rabies", GivenOn = new DateOnly(2026, 10, 1) });
await db.SaveChangesAsync();
Console.WriteLine($"   saved; plain query finds {await db.Vaccinations.CountAsync(v => v.PetId == 404)} row");
var withPet = await db.Vaccinations.Include(v => v.Pet).Where(v => v.PetId == 404).ToListAsync();
Console.WriteLine($"   Include(Pet) finds {withPet.Count} rows (required relationship = INNER JOIN)");

Console.WriteLine("2) visit for the same missing pet (FK kept):");
try
{
    db.Visits.Add(new Visit { PetId = 404, StartsAt = DateTime.UtcNow });
    await db.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    Console.WriteLine($"   rejected: {ex.InnerException?.Message}");
}

Console.WriteLine("3) the pet arrives late; the same record now resolves:");
db.ChangeTracker.Clear();
db.Pets.Add(new Pet { Id = 404, Name = "Biscuit", Species = "Dog" });
await db.SaveChangesAsync();
var resolved = await db.Vaccinations.Include(v => v.Pet).SingleAsync(v => v.PetId == 404);
Console.WriteLine($"   Pet navigation = {resolved.Pet?.Name}");
