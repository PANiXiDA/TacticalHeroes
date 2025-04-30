using Assets.Scripts.Common.Helpers;
using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.Services.Interfaces.Battle;

using TMPro;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class UnitView : MonoBehaviour
    {
        private const string TopMeleeAttackAnimationName = "TopMeleeAttack";
        private const string FrontMeleeAttackAnimationName = "FrontMeleeAttack";
        private const string BottomMeleeAttackAnimationName = "BottomMeleeAttack";
        private const string RangeAttackAnimationName = "RangeAttack";
        private const string TakeDamageAnimationName = "TakeDamage";
        private const string DeathAnimationName = "Death";
        private static readonly int IsMovingKey = Animator.StringToHash("Move");

        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Animator _animator;

        [SerializeField] private SpriteRenderer _countImage;
        [SerializeField] private TMP_Text _countText;

        [Inject] public Unit Data { get; private set; }
        [Inject] public PlayerSide Side { get; private set; }

        [Inject] private readonly IPlayerColorsService _playerColorsService;

        private void OnEnable()
        {
            SetCountData();
        }

        public void SetMovingState(bool isMoving) => _animator.SetBool(IsMovingKey, isMoving);
        public void PlayTopMeleeAttackAnimation() => _animator.Play(TopMeleeAttackAnimationName);
        public void PlayFrontMeleeAttackAnimation() => _animator.Play(FrontMeleeAttackAnimationName);
        public void PlayBottomMeleeAttackAnimation() => _animator.Play(BottomMeleeAttackAnimationName);
        public void PlayRangeAttackAnimation() => _animator.Play(RangeAttackAnimationName);
        public void PlayTakeDamageAnimation() => _animator.Play(TakeDamageAnimationName);
        public void PlayDeathAnimation() => _animator.Play(DeathAnimationName);

        public void IncrementSortingOrder() => _sprite.sortingOrder++;
        public void DecrementSortingOrder() => _sprite.sortingOrder--;
        public void Flip(bool value) => _sprite.flipX = value;

        public void SetCount(int count) => _countText.text = count.ToString();

        private void SetCountData()
        {
            if (Data.OwnerId.HasValue)
            {
                var color = _playerColorsService.GetRgb24(Data.OwnerId.Value).ToColor();
                _countImage.color = color;
            }
            _countText.text = Data.Count.ToString();
        }
    }
}
