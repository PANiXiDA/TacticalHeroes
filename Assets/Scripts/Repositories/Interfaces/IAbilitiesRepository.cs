using Assets.Scripts.Repositories.Interfaces.Core;

using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;

using Assets.Scripts.Domain.Entities.Models;

namespace Assets.Scripts.Repositories.Interfaces
{
    public interface IAbilitiesRepository : IBaseRepository<Ability, int, AbilitiesSearchParams, AbilitiesConvertParams>
    {
    }
}
