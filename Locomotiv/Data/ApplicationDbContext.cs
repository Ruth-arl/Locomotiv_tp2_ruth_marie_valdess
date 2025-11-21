using GMap.NET;
using Locomotiv.Model;
using Locomotiv.Model.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Définir le chemin absolu pour la base de données dans le répertoire AppData
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Locomotiv", "Locomotiv.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        var connectionString = $"Data Source={dbPath}";
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Station)
            .WithMany()
            .HasForeignKey(u => u.StationId)
        .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<Station>()
            .HasKey(s => s.IdStation);

        modelBuilder.Entity<Station>()
            .HasMany(s => s.Trains)
            .WithOne(t => t.Station)
            .HasForeignKey(t => t.IdStation)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Station>()
            .HasMany(s => s.Voies)
            .WithOne(v => v.Station)
            .HasForeignKey(v => v.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Station>()
            .HasMany(s => s.Signaux)
            .WithOne(sig => sig.Station)
            .HasForeignKey(sig => sig.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Train>()
            .HasKey(t => t.IdTrain);

        modelBuilder.Entity<Train>()
            .HasOne(t => t.Station)
            .WithMany(s => s.Trains)
            .HasForeignKey(t => t.IdStation)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Voie>()
            .HasOne(v => v.Station)
            .WithMany(s => s.Voies)
            .HasForeignKey(v => v.IdStation)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Signal>()
            .HasOne(sig => sig.Station)
            .WithMany(s => s.Signaux)
            .HasForeignKey(sig => sig.IdStation)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Block>()
            .HasOne(b => b.Station)
            .WithMany()
            .HasForeignKey(b => b.StationId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Voie>()
            .HasKey(v => v.Id);

        modelBuilder.Entity<Signal>()
            .HasKey(s => s.Id);

        modelBuilder.Entity<PointInteret>()
            .HasKey(p => p.Id);

        modelBuilder.Ignore<List<PointLatLng>>();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Train> Trains { get; set; }
    public DbSet<Voie> Voies { get; set; }
    public DbSet<Signal> Signals { get; set; }
    public DbSet<Block> Blocks { get; set; }

    public DbSet<PointInteret> PointsInteret { get; set; }

    public void SeedData()
    {
        try
        {
            Database.EnsureCreated();

            if (!Stations.Any())
            {
                var station1 = new Station { Nom = "Gare de Québec-Gatineau", Ville = "Québec", CapaciteMax = 10, Latitude = 76.82, Longitude = -71.19 };
                var station2 = new Station { Nom = "Gare du Palais", Ville = "Montréal", CapaciteMax = 12, Latitude = 46.82, Longitude = -78.19 };
                var station3 = new Station { Nom = "Gare CN", Ville = "Québec", CapaciteMax = 15, Latitude = 56.82, Longitude = -71.19 };
                Stations.AddRange(station1, station2, station3);
                SaveChanges();

                Voies.AddRange(
                    new Voie { Nom = "Voie A", IdStation = station1.IdStation },
                    new Voie { Nom = "Voie B", IdStation = station1.IdStation },
                    new Voie { Nom = "Voie 1", IdStation = station2.IdStation }
                );

                Signals.AddRange(
                    new Signal { Type = "Entrée", Etat = SignalState.Vert, IdStation = station1.IdStation },
                    new Signal { Type = "Sortie", Etat = SignalState.Rouge, IdStation = station1.IdStation },
                    new Signal { Type = "Entrée", Etat = SignalState.Jaune, IdStation = station2.IdStation }
                );
                SaveChanges();
            }
           
            var s1 = Stations.FirstOrDefault(s => s.Nom == "Gare de Québec-Gatineau");
            var s2 = Stations.FirstOrDefault(s => s.Nom == "Gare du Palais");
            var s3 = Stations.FirstOrDefault(s => s.Nom == "Gare CN");


            if (!Users.Any())
            {
                Users.AddRange(
                    new User
                    {
                        Prenom = "John",
                        Nom = "Doe",
                        Username = "johndoe",
                        Password = "password123",
                        Station = s1,
                        Role = UserRole.Employe
                    },
                    new User
                    {
                        Prenom = "Jane",
                        Nom = "Doe",
                        Username = "janedoe",
                        Password = "password123",
                        Station = s2,
                        Role = UserRole.Employe
                    },
                    new User
                    {
                        Prenom = "Admin",
                        Nom = "System",
                        Username = "admin",
                        Password = "admin123",
                        Role = UserRole.Administrateur,
                        Station = s1,
                        StationId = s1.IdStation
                    },
                    new User
                    {
                        Prenom = "Admin",
                        Nom = "System",
                        Username = "admin1",
                        Password = "admin123",
                        Role = UserRole.Administrateur,
                        Station = s2,
                        StationId = s2.IdStation
                    },
                    new User
                    {
                        Prenom = "Admin",
                        Nom = "System",
                        Username = "admin2",
                        Password = "admin123",
                        Role = UserRole.Administrateur,
                        Station = s3,
                        StationId = s3.IdStation
                    }
                );
                SaveChanges();
            }

            if (!Trains.Any())
            {
                Trains.AddRange(
                    new Train
                    {
                        IdStation = s1!.IdStation,
                        Type = TrainType.Express,
                        Etat = TrainStatus.EnGare,
                        EstEnGare = true
                    },
                    new Train
                    {
                        IdStation = s2!.IdStation,
                        Type = TrainType.Passagers,
                        Etat = TrainStatus.EnGare,
                        EstEnGare = true
                    },
                    new Train
                    {
                        IdStation = s3.IdStation,
                        Station = s3,
                        Type = TrainType.Maintenance,
                        Etat = TrainStatus.EnGare
                    }
                );
                SaveChanges();
            }

            if (!PointsInteret.Any())
            {
                PointsInteret.AddRange(
                    new PointInteret { Nom = "Vers Charlevoix", Type = "Destination", Latitude = 46.82, Longitude = -71.19 },
                    new PointInteret { Nom = "Baie de Beauport", Type = "Destination", Latitude = 46.85, Longitude = -71.18 },
                    new PointInteret { Nom = "Port de Québec", Type = "Destination", Latitude = 46.81, Longitude = -71.20 },
                    new PointInteret { Nom = "Centre de distribution", Type = "Logistique", Latitude = 46.80, Longitude = -71.23 },
                    new PointInteret { Nom = "Vers la rive-sud", Type = "Destination", Latitude = 46.78, Longitude = -71.26 },
                    new PointInteret { Nom = "Vers Gatineau", Type = "Destination", Latitude = 46.79, Longitude = -71.28 },
                    new PointInteret { Nom = "Vers le nord", Type = "Destination", Latitude = 46.85, Longitude = -71.24 }
                );
                SaveChanges();
            }

            if (!Blocks.Any())
            {
                Blocks.AddRange(
                    new Block { Nom = "B1-Palais", StationId = 1, EtatSignal = SignalState.Vert },
                    new Block { Nom = "B2-Connexion", StationId = null, EtatSignal = SignalState.Rouge },
                    new Block { Nom = "B3-Gatineau", StationId = 2, EtatSignal = SignalState.Rouge },
                    new Block { Nom = "B4-Connexion", StationId = null, EtatSignal = SignalState.Jaune },
                    new Block { Nom = "B5-CN", StationId = 3, EtatSignal = SignalState.Vert }
                );
                SaveChanges();
            }

            if (!Voies.Any())
            {
                Voies.AddRange(
                    new Voie { Nom = "Q1", IdStation = s1.IdStation, Station = s1 },
                    new Voie { Nom = "Q2", IdStation = s2.IdStation, Station = s2 },
                    new Voie { Nom = "Q3", IdStation = s3.IdStation, Station = s3 }
                );
                SaveChanges();
            }


            if (!Signals.Any())
            {
                Signals.AddRange(
                    new Signal { Type = "Entrée", Etat = SignalState.Vert, IdStation = s1.IdStation },
                    new Signal { Type = "Sortie", Etat = SignalState.Vert, IdStation = s1.IdStation },
                    new Signal { Type = "Entrée", Etat = SignalState.Rouge, IdStation = s2.IdStation },
                    new Signal { Type = "Sortie", Etat = SignalState.Vert, IdStation = s3.IdStation },
                    new Signal { Type = "Entrée", Etat = SignalState.Jaune, IdStation = s3.IdStation },
                    new Signal { Type = "Sortie", Etat = SignalState.Vert, IdStation = s2.IdStation }
                );
                SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur ...");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
            throw;
        }     
    }
}