using System;
using System.Collections.Generic;

namespace Assets.Scripts.Domain.Entities.Models
{
    public class Build
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<int> UnitIds { get; set; } = new();

        public List<Unit> Units { get; set; } = new();

        public Build(
            Guid id,
            string name,
            List<int> unitIds)
        {
            Id = id;
            Name = name;
            UnitIds = unitIds;
        }
    }
}
