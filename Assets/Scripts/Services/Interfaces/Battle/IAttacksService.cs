using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IAttacksService
    {
        UniTask MeleeAttackAsync(Unit attacker, Unit defender, Tile targetTile);
        UniTask RangeAttackAsync(GameObject attacker, Unit defender);
    }
}
