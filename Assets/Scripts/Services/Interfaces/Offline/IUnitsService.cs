using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Domain.Entities.Models;
using Assets.Scripts.Services.Interfaces.Core;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IUnitsService : ICrudService<Unit, int, UnitsSearchParams, UnitsConvertParams>
    {
    }
}
