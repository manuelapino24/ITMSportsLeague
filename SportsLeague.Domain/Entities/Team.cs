using System;
using System.Collections.Generic;

namespace SportsLeague.Domain.Entities
{
    public class Team : AuditBase
    {
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Stadium { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public DateTime FoundedDate { get; set; }

        // Corregido: Players ahora es una colección tipada compatible con EF Core
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
