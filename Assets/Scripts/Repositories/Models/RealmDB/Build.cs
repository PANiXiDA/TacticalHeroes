using System;
using System.Collections.Generic;

using Realms;

namespace Assets.Scripts.Repositories.Models.RealmDB
{
    public partial class Build : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<UnitInBuild> Units { get; }
    }

    public partial class UnitInBuild : EmbeddedObject
    {
        public int UnitId { get; set; }
        public int Amount { get; set; }
    }
}
