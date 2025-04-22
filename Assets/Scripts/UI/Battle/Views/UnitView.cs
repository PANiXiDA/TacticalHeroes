using Assets.Scripts.GameEngine.Domain;

using UnityEngine;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class UnitView : MonoBehaviour
    {
        private const string MoveAnimationName = "Move";
        private const string TopMeleeAttackAnimationName = "TopMeleeAttack";
        private const string FrontMeleeAttackAnimationName = "FrontMeleeAttack";
        private const string BottomMeleeAttackAnimationName = "BottomMeleeAttack";
        private const string RangeAttackAnimationName = "RangeAttack";
        private const string TakeDamageAnimationName = "TakeDamage";
        private const string DeathAnimationName = "Death";

        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Animator _animator;

        public Unit Data { get; private set; }
        public Side Side { get; private set; }

        public void Init(Unit unit, Side side)
        {
            Data = unit;
            Side = side;
        }

        public void PlayMoveAnimation() => _animator.Play(MoveAnimationName);
        public void PlayTopMeleeAttackAnimation() => _animator.Play(TopMeleeAttackAnimationName);
        public void PlayFrontMeleeAttackAnimation() => _animator.Play(FrontMeleeAttackAnimationName);
        public void PlayBottomMeleeAttackAnimation() => _animator.Play(BottomMeleeAttackAnimationName);
        public void PlayRangeAttackAnimation() => _animator.Play(RangeAttackAnimationName);
        public void PlayTakeDamageAnimation() => _animator.Play(TakeDamageAnimationName);
        public void PlayDeathAnimation() => _animator.Play(DeathAnimationName);
    }
}
