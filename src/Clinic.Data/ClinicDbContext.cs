using Microsoft.EntityFrameworkCore;

namespace Clinic.Data;

public class ClinicDbContext(DbContextOptions<ClinicDbContext> options) : DbContext(options)
{
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<VaccinationRecord> Vaccinations => Set<VaccinationRecord>();
    public DbSet<Visit> Visits => Set<Visit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Still a relationship in the model (Include, tracking), but no FOREIGN KEY in the migration.
        modelBuilder.Entity<VaccinationRecord>()
            .HasOne(v => v.Pet)
            .WithMany()
            .HasForeignKey(v => v.PetId)
            .ExcludeForeignKeyFromMigrations();

        // Default behaviour: the database enforces this one.
        modelBuilder.Entity<Visit>()
            .HasOne(v => v.Pet)
            .WithMany()
            .HasForeignKey(v => v.PetId);
    }
}

public class ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : DbContext(options)
{
    public DbSet<OnCallShift> Shifts => Set<OnCallShift>();
}
