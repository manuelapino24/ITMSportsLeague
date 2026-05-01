using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Interfaces;

using SportsLeague.Domain.Entities;

using SportsLeague.Domain.Interfaces.Repositories;


namespace SportsLeague.Domain.Interfaces.Repositories;



public interface IMatchResultRepository : IGenericRepository<MatchResult>

{

    Task<MatchResult?> GetByMatchIdAsync(int matchId);

}