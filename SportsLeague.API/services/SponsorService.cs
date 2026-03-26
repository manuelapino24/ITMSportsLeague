using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Text.RegularExpressions;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
        ISponsorRepository sponsorRepository,
        ITournamentSponsorRepository tournamentSponsorRepository,
        ITournamentRepository tournamentRepository,
        ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _tournamentRepository = tournamentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all sponsors");
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
        var sponsor = await _sponsorRepository.GetByIdAsync(id);
        if (sponsor == null)
            _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
        return sponsor;
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        // Validate duplicate name
        var nameExists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
        if (nameExists)
            throw new InvalidOperationException(
                $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");

        // Validate email format
        if (!IsValidEmail(sponsor.ContactEmail))
            throw new InvalidOperationException(
                $"El formato del email '{sponsor.ContactEmail}' no es válido");

        _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");

        // Validate duplicate name (if changed)
        if (existing.Name != sponsor.Name)
        {
            var nameExists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (nameExists)
                throw new InvalidOperationException(
                    $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
        }

        // Validate email format
        if (!IsValidEmail(sponsor.ContactEmail))
            throw new InvalidOperationException(
                $"El formato del email '{sponsor.ContactEmail}' no es válido");

        existing.Name = sponsor.Name;
        existing.ContactEmail = sponsor.ContactEmail;
        existing.Phone = sponsor.Phone;
        existing.WebsiteUrl = sponsor.WebsiteUrl;
        existing.Category = sponsor.Category;

        _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _sponsorRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");

        _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.DeleteAsync(id);
    }

    public async Task LinkToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
    {
        // Validate that the sponsor exists
        var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
        if (!sponsorExists)
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");

        // Validate that the tournament exists
        var tournamentExists = await _tournamentRepository.ExistsAsync(tournamentId);
        if (!tournamentExists)
            throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentId}");

        // Validate duplicate
        var existing = await _tournamentSponsorRepository
            .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existing != null)
            throw new InvalidOperationException(
                "Este patrocinador ya está vinculado a este torneo");

        // Validate ContractAmount > 0
        if (contractAmount <= 0)
            throw new InvalidOperationException(
                "El monto del contrato debe ser mayor a 0");

        var tournamentSponsor = new TournamentSponsor
        {
            TournamentId = tournamentId,
            SponsorId = sponsorId,
            ContractAmount = contractAmount,
            JoinedAt = DateTime.UtcNow
        };

        _logger.LogInformation(
            "Linking sponsor {SponsorId} to tournament {TournamentId}",
            sponsorId, tournamentId);
        await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
    }

    public async Task UnlinkFromTournamentAsync(int sponsorId, int tournamentId)
    {
        var existing = await _tournamentSponsorRepository
            .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existing == null)
            throw new KeyNotFoundException(
                "No existe vinculación entre este patrocinador y el torneo");

        _logger.LogInformation(
            "Unlinking sponsor {SponsorId} from tournament {TournamentId}",
            sponsorId, tournamentId);
        await _tournamentSponsorRepository.DeleteAsync(existing.Id);
    }

    public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        var exists = await _sponsorRepository.ExistsAsync(sponsorId);
        if (!exists)
            throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");

        return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
    }

    private static bool IsValidEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }
}