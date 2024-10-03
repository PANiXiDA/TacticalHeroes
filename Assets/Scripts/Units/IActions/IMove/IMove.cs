using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.IActions
{
    public interface IMove
    {
        public UniTask Move(BaseUnit unit, Tile targetTile);
    }
}
