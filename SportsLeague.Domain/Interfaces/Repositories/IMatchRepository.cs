using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(int id);
    Task<IEnumerable<Match>> GetByTournamentAsync(int tournamentId);
    Task<Match?> GetWithDetailsAsync(int matchId);
}