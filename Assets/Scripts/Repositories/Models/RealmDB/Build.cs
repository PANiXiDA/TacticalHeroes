using System;
using System.Collections.Generic;

using Realms;

namespace Assets.Scripts.Repositories.Models.RealmDB
{
    public class Build : RealmObject
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<int> UnitIds { get; }
    }
}
