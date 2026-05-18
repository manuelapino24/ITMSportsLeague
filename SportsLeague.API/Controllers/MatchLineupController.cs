using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/match/{matchId}/lineup")]
public class MatchLineupController : ControllerBase
{
    private readonly IMatchLineupService _lineupService;
    private readonly IMapper _mapper;

    public MatchLineupController(IMatchLineupService lineupService, IMapper mapper)
    {
        _lineupService = lineupService;
        _mapper = mapper;
    }

    // POST /api/match/{matchId}/lineup
    [HttpPost]
    public async Task<ActionResult<MatchLineupDto>> AddPlayer(
        int matchId, CreateMatchLineupDto dto)
    {
        try
        {
            var lineup = _mapper.Map<MatchLineup>(dto);
            var created = await _lineupService.AddPlayerAsync(matchId, lineup);

            // Recargar con detalles para el response
            var all = await _lineupService.GetByMatchAsync(matchId);
            var createdWithDetails = all.FirstOrDefault(l => l.Id == created.Id);

            return StatusCode(201, _mapper.Map<MatchLineupDto>(createdWithDetails));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    // GET /api/match/{matchId}/lineup
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetLineup(int matchId)
    {
        try
        {
            var lineup = await _lineupService.GetByMatchAsync(matchId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupDto>>(lineup));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    // GET /api/match/{matchId}/lineup/team/{teamId}
    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<IEnumerable<MatchLineupDto>>> GetLineupByTeam(
        int matchId, int teamId)
    {
        try
        {
            var lineup = await _lineupService.GetByMatchAndTeamAsync(matchId, teamId);
            return Ok(_mapper.Map<IEnumerable<MatchLineupDto>>(lineup));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    // DELETE /api/match/{matchId}/lineup/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePlayer(int matchId, int id)
    {
        try
        {
            await _lineupService.DeleteAsync(matchId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}
