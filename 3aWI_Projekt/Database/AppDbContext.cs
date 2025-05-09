using Microsoft.EntityFrameworkCore;
using _3aWI_Projekt.Models;

namespace _3aWI_Projekt.Database;

public class AppDbContext : DbContext
{
    public DbSet<School> Schools => Set<School>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();

    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ---------- Primärschlüssel ----------
        mb.Entity<School>().HasKey(s => s.Id);
        mb.Entity<Student>().HasKey(s => s.Id);
        mb.Entity<Classroom>().HasKey(c => c.Id);

        // ---------- Beziehungen ----------
        mb.Entity<School>()
          .HasMany(s => s.Students)
          .WithOne()
          .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<School>()
          .HasMany(s => s.Classrooms)
          .WithOne()
          .OnDelete(DeleteBehavior.Cascade);

        // Classroom erbt von School → Rekursion verhindern
        mb.Entity<Classroom>()
          .Ignore(c => c.Classrooms)
          .Ignore(c => c.Students);  // falls doppelt vorhanden
    }
}
