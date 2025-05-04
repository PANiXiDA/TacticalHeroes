using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IAttacksService
    {
        UniTask MeleeAttackAsync(Unit attacker, Unit defender, Tile targetTile);
    }
}
