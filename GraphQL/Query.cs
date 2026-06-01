using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tapi.Data;
using Tapi.Models;


namespace Tapi.GraphQL;

public class Query
{
    public Task<List<Tournament>> GetTournaments([Service] AppDbContext context) =>
        context.Tournaments.Include(t => t.Bracket).ThenInclude(b => b.Matches).ToListAsync();

    [Authorize]
    public async Task<List<Match>> GetMyMatches([Service] AppDbContext context, ClaimsPrincipal claimsPrincipal)
    {
        
        var userIdStr = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
        {
            throw new GraphQLException("Błąd autoryzacji");
        }

        return await context.Matches
            .Where(m => m.Player1Id == userId || m.Player2Id == userId)
            .Include(m => m.Player1)
            .Include(m => m.Player2)
            .Include(m => m.Winner)
            .ToListAsync();
    }
}


