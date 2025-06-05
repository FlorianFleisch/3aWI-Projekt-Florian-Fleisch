using Microsoft.EntityFrameworkCore;
using _3aWI_Projekt.Models;

namespace _3aWI_Projekt.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database\\app.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            optionsBuilder.LogTo(log => { Console.WriteLine(log); },LogLevel.Error);
        }
    }
}
