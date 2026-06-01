using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Tapi.Data;
using Tapi.Models;
using Tapi.Services;
using HotChocolate;

namespace Tapi.GraphQL;

public class Mutation
{
    public async Task<User> Register([Service] AppDbContext context, string firstName, string lastName, string email, string password)
    {
        var user = new User { FirstName = firstName, LastName = lastName, Email = email, PasswordHash = password };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<string> Login([Service] AppDbContext context, [Service] AuthService authService, string email, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        if (user == null) throw new GraphQLException("Błędne dane .");
        return authService.GenerateToken(user);
    }

    public async Task<Tournament> AddParticipant([Service] AppDbContext context, int tournamentId, int userId)
    {
        var tournament = await context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
        if (tournament == null) throw new GraphQLException("Brak turnieju!");

        return tournament;
    }

    public async Task<Tournament> StartTournament([Service] AppDbContext context, int tournamentId)
    {
        var tournament = await context.Tournaments.Include(t => t.Bracket).FirstOrDefaultAsync(t => t.Id == tournamentId);
        if (tournament == null) throw new GraphQLException("Brak turnieju!");

        tournament.Status = "Start";

        var users = await context.Users.Take(2).ToListAsync();
        var bracket = new Bracket();

        if (users.Count >= 2)
        {
            bracket.Matches.Add(new Match { Round = 1, Player1 = users[0], Player2 = users[1] });
        }

        tournament.Bracket = bracket;
        await context.SaveChangesAsync();
        return tournament;
    }

    public async Task<Tournament> FinishTournament([Service] AppDbContext context, int tournamentId)
    {
        var tournament = await context.Tournaments.FirstOrDefaultAsync(t => t.Id == tournamentId);
        if (tournament == null) throw new GraphQLException("Turniej nie istnieje");

        tournament.Status = "Koniec";
        await context.SaveChangesAsync();
        return tournament;
    }

    [GraphQLName("getMatchesForRound")]
    public async Task<List<Match>> GetMatchesForRound([Service] AppDbContext context, int round)
    {
        return await context.Matches
            .Where(m => m.Round == round)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .ToListAsync();
    }

    public async Task<Match> PlayMatch([Service] AppDbContext context, int matchId, int winnerUserId)
    {
        var match = await context.Matches
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null) throw new GraphQLException("Brak meczu.");

        
        if ((match.Player1 == null || match.Player1.Id != winnerUserId) &&
            (match.Player2 == null || match.Player2.Id != winnerUserId))
        {
            throw new GraphQLException(" użytkownik nie jest uczestnikiem .");
        }

        var winnerUser = await context.Users.FindAsync(winnerUserId);
        if (winnerUser == null) throw new GraphQLException("Użytkownik nie istnieje.");

        match.Winner = winnerUser;
        await context.SaveChangesAsync();

        return match;
    }
}