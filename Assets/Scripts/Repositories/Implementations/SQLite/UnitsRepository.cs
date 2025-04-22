using Assets.Scripts.Repositories.Implementations.SQLite.Core;
using Assets.Scripts.Repositories.Interfaces;

using SQLite4Unity3d;

namespace Assets.Scripts.Repositories.Implementations.SQLite
{
    public class UnitsRepository : IUnitsRepository
    {
        private readonly SQLiteConnection _db;

        public UnitsRepository(DatabaseContext dbService)
        {
            _db = dbService.Connection;
        }
    }
}
