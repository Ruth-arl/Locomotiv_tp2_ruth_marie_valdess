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
            .HasForeignKey(v => v.IdStation)
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


        modelBuilder.Entity<Signal>()
            .HasOne(sig => sig.Station)
            .WithMany(s => s.Signaux)
            .HasForeignKey(sig => sig.IdStation)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Block>()
            .HasKey(b => b.IdBlock);

        modelBuilder.Entity<Block>()
           .Property(b => b.EtatSignal)
           .HasConversion<string>();

        modelBuilder.Entity<Voie>()
            .HasKey(v => v.IdVoie);

        modelBuilder.Entity<Voie>()
            .HasOne(v => v.Station)
            .WithMany(s => s.Voies)
            .HasForeignKey(v => v.IdStation)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Signal>()
           .Property(s => s.EtatSignal)
           .HasConversion<string>();

        modelBuilder.Entity<PointInteret>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Itineraire>()
            .HasKey(i => i.IdItineraire);

        modelBuilder.Entity<Itineraire>()
            .HasOne(i => i.Train)
            .WithMany()
            .HasForeignKey(i => i.IdTrain)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ArretItineraire>()
            .HasKey(a => a.IdArret);

        modelBuilder.Entity<ArretItineraire>()
            .HasOne(a => a.Itineraire)
            .WithMany(i => i.Arrets)
            .HasForeignKey(a => a.IdItineraire)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Train> Trains { get; set; }
    public DbSet<Voie> Voies { get; set; }
    public DbSet<Signal> Signals { get; set; }
    public DbSet<Block> Blocks { get; set; }

    public DbSet<PointInteret> PointsInteret { get; set; }
    public DbSet<Itineraire> Itineraires { get; set; }
    public DbSet<ArretItineraire> ArretsItineraire { get; set; }

    public void SeedData()
    {
        try
        {
            Database.EnsureCreated();

            if (!Stations.Any())
            {
                var station1 = new Station { Nom = "Gare de Québec-Gatineau", Ville = "Québec", CapaciteMax = 10, Latitude = 46.82, Longitude = -71.19 };
                var station2 = new Station { Nom = "Gare du Palais", Ville = "Montréal", CapaciteMax = 12, Latitude = 46.82, Longitude = -78.19 };
                var station3 = new Station { Nom = "Gare CN", Ville = "Québec", CapaciteMax = 15, Latitude = 56.82, Longitude = -71.19 };
                Stations.AddRange(station1, station2, station3);
                SaveChanges();

                Voies.AddRange(
                    new Voie { Nom = "Voie A", IdStation = station1.IdStation, EstOccupee = false },
                    new Voie { Nom = "Voie B", IdStation = station1.IdStation, EstOccupee = false },
                    new Voie { Nom = "Voie 1", IdStation = station2.IdStation, EstOccupee = false },
                    new Voie { Nom = "Voie 1", IdStation = station2.IdStation, EstOccupee = false },
                    new Voie { Nom = "Voie 2", IdStation = station2.IdStation, EstOccupee = false },
                    new Voie { Nom = "Voie 1", IdStation = station3.IdStation, EstOccupee = false },
                    new Voie { Nom = "Voie 2", IdStation = station3.IdStation, EstOccupee = false }
                );
                SaveChanges();

                Signals.AddRange(
                    new Signal { Type = "Entrée", EtatSignal = SignalState.Vert, IdStation = station1.IdStation },
                    new Signal { Type = "Sortie", EtatSignal = SignalState.Rouge, IdStation = station1.IdStation },
                    new Signal { Type = "Entrée", EtatSignal = SignalState.Jaune, IdStation = station2.IdStation }
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
                        Station = null,
                        StationId = null
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
                        HeureDepart = DateTime.Now.AddHours(1),
                        HeureArrivee = DateTime.Now.AddMinutes(30),
                        Type = TrainType.Express,
                        Etat = TrainStatus.EnGare,
                        EstEnGare = true
                    },
                    new Train
                    {
                        IdStation = s2!.IdStation,
                        HeureDepart = DateTime.Now.AddHours(2),
                        HeureArrivee = DateTime.Now.AddMinutes(15),
                        Type = TrainType.Passagers,
                        Etat = TrainStatus.EnGare,
                        EstEnGare = true
                    },
                    new Train
                    {
                        IdStation = s3.IdStation,
                        Station = s3,
                        HeureDepart = DateTime.Now.AddHours(2),
                        HeureArrivee = DateTime.Now.AddHours(4),
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

            if (!Blocks.Any() && s1 != null && s2 != null && s3 != null)
            {
                Blocks.AddRange(
                new Block
                {
                    Nom = "Block A1-A2a",
                    LatitudeDebut = 46.8400,
                    LongitudeDebut = -71.2950,
                    LatitudeFin = 46.8350,
                    LongitudeFin = -71.2750,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },
                new Block
                {
                    Nom = "Block A2b-A3",
                    LatitudeDebut = 46.8350,
                    LongitudeDebut = -71.2750,
                    LatitudeFin = 46.8300,
                    LongitudeFin = -71.2550,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },
                new Block
                {
                    Nom = "Block A3-B1",
                    LatitudeDebut = 46.8300,
                    LongitudeDebut = -71.2550,
                    LatitudeFin = 46.8200,
                    LongitudeFin = -71.2250,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },

                new Block
                {
                    Nom = "Block B1-B2",
                    LatitudeDebut = 46.8200,
                    LongitudeDebut = -71.2250,
                    LatitudeFin = 46.8000,
                    LongitudeFin = -71.2600,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },
                new Block
                {
                    Nom = "Block B2-C1",
                    LatitudeDebut = 46.8000,
                    LongitudeDebut = -71.2600,
                    LatitudeFin = 46.7850,
                    LongitudeFin = -71.2900,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },
                new Block
                {
                    Nom = "Block C1-C2",
                    LatitudeDebut = 46.7850,
                    LongitudeDebut = -71.2900,
                    LatitudeFin = 46.7650,
                    LongitudeFin = -71.3200,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },
                new Block
                {
                    Nom = "Block B-Charlevoix",
                    LatitudeDebut = 46.8200,
                    LongitudeDebut = -71.2250,
                    LatitudeFin = 46.8500,
                    LongitudeFin = -71.1900,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },
                new Block
                {
                    Nom = "Block A-Gatineau",
                    LatitudeDebut = 46.8400,
                    LongitudeDebut = -71.2950,
                    LatitudeFin = 46.8500,
                    LongitudeFin = -71.3200,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },

                new Block
                {
                    Nom = "Block A-Nord",
                    LatitudeDebut = 46.8400,
                    LongitudeDebut = -71.2950,
                    LatitudeFin = 46.8700,
                    LongitudeFin = -71.2400,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                },

                new Block
                {
                    Nom = "Block C-RiveSud",
                    LatitudeDebut = 46.7650,
                    LongitudeDebut = -71.3200,
                    LatitudeFin = 46.7600,
                    LongitudeFin = -71.3400,
                    EtatSignal = SignalState.Vert,
                    TrainOccupantId = null
                });
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