using SportsLeague.Domain.Enums;



namespace SportsLeague.Domain.Entities;



public class Card : AuditBase

{

    public int MatchId { get; set; }

    public int PlayerId { get; set; }

    public int Minute { get; set; }

    public CardType Type { get; set; }



    // Navigation Properties 

    public Match Match { get; set; } = null!;

    public Player Player { get; set; } = null!;

    // Relación 1:1 con resultado 

    public MatchResult? MatchResult { get; set; }



    // Relación 1:N con goles y tarjetas 

    public ICollection<Goal> Goals { get; set; } = new List<Goal>();

    public ICollection<Card> Cards { get; set; } = new List<Card>();

}