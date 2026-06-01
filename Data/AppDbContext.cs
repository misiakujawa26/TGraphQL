using Microsoft.EntityFrameworkCore;
using Tapi.Models;


namespace Tapi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Bracket> Brackets => Set<Bracket>();
    public DbSet<Match> Matches => Set<Match>();
    public void SeedData()
    {

        if (!Users.Any())
        {
            var user1 = new User { Id = 1, FirstName = "Adam", LastName = "Nowak", Email = "adam@nowak.pl", PasswordHash = "tajne" };
            var user2 = new User { Id = 2, FirstName = "Ewa", LastName = "Kowalska", Email = "ewa@kowalska.pl", PasswordHash = "123" };

            Users.AddRange(user1, user2);

            var tournament = new Tournament
            {
                Id = 1,
                Name = "Mecz",
                StartDate = DateTime.UtcNow,
                Status = "Created",
                Bracket = new Bracket()
            };

            Tournaments.Add(tournament);
            SaveChanges();
        }
    }
}




