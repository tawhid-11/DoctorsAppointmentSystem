using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; } 
    public DbSet<SlotTemplate> SlotTemplates { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prevent double-booking: unique index on date + slot
        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.AppointmentDate, a.SlotTemplateId })
            .IsUnique();
    }
}
