using System;
using System.Collections.Generic;

namespace Assets.Scripts.Domain.Entities.Models
{
    public class Build
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Dictionary<int, int> UnitIdsAndCounts { get; set; } = new();

        public List<Unit> Units { get; set; } = new();

        public Build(
            Guid id,
            string name,
            Dictionary<int, int> unitIdsAndCounts)
        {
            Id = id;
            Name = name;
            UnitIdsAndCounts = unitIdsAndCounts;
        }
    }
}
