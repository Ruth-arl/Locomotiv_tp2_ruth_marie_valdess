using Locomotiv.Model;
using Locomotiv.Model.Enums;
using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;

public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(
       DbContextOptionsBuilder optionsBuilder)
    {
        // Définir le chemin absolu pour la base de données dans le répertoire AppData
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Locomotiv", "Locomotiv.db");

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
        var connectionString = $"Data Source={dbPath}";

        // Configurer le DbContext pour utiliser la chaîne de connexion
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Station>()
            .HasKey(s => s.IdStation);

        modelBuilder.Entity<Train>()
            .HasKey(t => t.IdTrain);

        modelBuilder.Entity<Train>()
            .HasOne(t => t.Station)
            .WithMany(s => s.Trains)
            .HasForeignKey(t => t.IdStation)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Train> Trains { get; set; }

    public void SeedData()
    {
        Database.EnsureCreated();

        if (!Stations.Any())
        {
            var station1 = new Station { Nom = "Gare de Québec", Ville = "Québec", CapaciteMax = 10 };
            var station2 = new Station { Nom = "Gare de Montréal", Ville = "Montréal", CapaciteMax = 12 };
            Stations.AddRange(station1, station2);
            SaveChanges();
        }

        var s1 = Stations.FirstOrDefault(s => s.Nom == "Gare de Québec");
        var s2 = Stations.FirstOrDefault(s => s.Nom == "Gare de Montréal");

        if (!Users.Any())
        {
            Users.AddRange(
                new User { Prenom = "John", Nom = "Doe", Username = "johndoe", Password = "password123", Station = s1, Role = UserRole.Employe },
                new User { Prenom = "Jane", Nom = "Doe", Username = "janedoe", Password = "password123", Station = s2, Role = UserRole.Employe },
                new User { Prenom = "Admin", Nom = "System", Username = "admin", Password = "admin123", Role = UserRole.Administrateur, Station = null, StationId=null }
            );
            SaveChanges();
        }

        if (!Trains.Any())
        {
            Trains.AddRange(
                new Train
                {
                    IdStation = s1.IdStation,
                    Station = s1,
                    HeureDepart = DateTime.Now.AddHours(1),
                    HeureArrivee = DateTime.Now.AddHours(3),
                    Type = TrainType.Express,
                    Etat = TrainStatus.EnGare
                },
                new Train
                {
                    IdStation = s2.IdStation,
                    Station = s2,
                    HeureDepart = DateTime.Now.AddHours(2),
                    HeureArrivee = DateTime.Now.AddHours(4),
                    Type = TrainType.Passagers,
                    Etat = TrainStatus.EnGare
                }
            );
            SaveChanges();
        }
    }
}
