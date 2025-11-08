using Locomotiv.Model;
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

    public DbSet<User> Users { get; set; }
    public DbSet<Station> Stations { get; set; }

    public void SeedData()
    {
        Database.EnsureCreated();

        if (!Stations.Any())
        {
            var station1 = new Station { Nom = "Gare de Québec", Ville = "Québec" };
            var station2 = new Station { Nom = "Gare de Montréal", Ville = "Montréal" };
            Stations.AddRange(station1, station2);
            SaveChanges();
        }

        var s1 = Stations.FirstOrDefault(s => s.Nom == "Gare de Québec");
        var s2 = Stations.FirstOrDefault(s => s.Nom == "Gare de Montréal");

        if (!Users.Any())
        {
            Users.AddRange(
                new User { Prenom = "John", Nom = "Doe", Username = "johndoe", Password = "password123", Station = s1 },
                new User { Prenom = "Jane", Nom = "Doe", Username = "janedoe", Password = "password123", Station = s2 }
            );

            SaveChanges();
        }
    }

}
