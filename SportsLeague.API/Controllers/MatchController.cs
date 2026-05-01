using Microsoft.AspNetCore.Mvc;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Enums;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly IMatchRepository _matchRepository;

    public MatchController(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    [HttpGet("tournament/{tournamentId}")]
    public async Task<IActionResult> GetByTournament(int tournamentId)
    {
        var matches = await _matchRepository.GetByTournamentAsync(tournamentId);
        return Ok(matches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var match = await _matchRepository.GetWithDetailsAsync(id);
        if (match == null) return NotFound();
        return Ok(match);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Match match)
    {
        var created = await _matchRepository.CreateAsync(match);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Match match)
    {
        match.Id = id;
        await _matchRepository.UpdateAsync(match);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _matchRepository.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] MatchStatus status)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match == null) return NotFound();
        match.Status = status;
        await _matchRepository.UpdateAsync(match);
        return NoContent();
    }
}