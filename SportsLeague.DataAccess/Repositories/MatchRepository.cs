using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class MatchRepository : GenericRepository<Match>, IMatchRepository
{
    public MatchRepository(LeagueDbContext context) : base(context) { }

    public async Task<IEnumerable<Match>> GetByTournamentAsync(int tournamentId)
    {
        return await _dbSet
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<Match?> GetWithDetailsAsync(int matchId)
    {
        return await _dbSet
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Referee)
            .Include(m => m.Goals)
            .Include(m => m.Cards)
            .Include(m => m.MatchResult)
            .FirstOrDefaultAsync(m => m.Id == matchId);
    }
}