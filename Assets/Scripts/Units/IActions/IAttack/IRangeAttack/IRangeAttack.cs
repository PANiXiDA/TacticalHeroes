using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.IActions
{
    public interface IRangeAttack
    {
        public UniTask RangeAttack(BaseUnit attacker, BaseUnit defender);
    }
}
