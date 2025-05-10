using System;
using System.Linq;

using Assets.Scripts.Common.Helpers;
using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using TMPro;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class UnitView : MonoBehaviour
    {
        private const float MoveSpeed = 5f;

        private const string TopMeleeAttackAnimationName = "TopMeleeAttack";
        private const string FrontMeleeAttackAnimationName = "FrontMeleeAttack";
        private const string BottomMeleeAttackAnimationName = "BottomMeleeAttack";
        private const string RangeAttackAnimationName = "RangeAttack";
        private const string TakeDamageAnimationName = "TakeDamage";
        private const string DeathAnimationName = "Death";
        private static readonly int IsMovingKey = Animator.StringToHash("Move");

        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider2D _collider2D;

        [SerializeField] private SpriteRenderer _countContainer;
        [SerializeField] private SpriteRenderer _countImage;
        [SerializeField] private TMP_Text _countText;

        [Inject] public Unit Data { get; private set; }
        [Inject] public PlayerSide Side { get; private set; }

        [Inject] private readonly IPlayerColorsService _playerColorsService;

        private bool _defaultFlip;

        private void OnEnable()
        {
            SetCountData();
            SetFlip();
        }

        public async UniTask MoveAsync(Vector3[] path, Action onComplete)
        {
            Face(path.First(), path.Last());
            SetMovingState(true);
            IncrementSortingOrder();
            float duration = CalculateMovementDuration(path);

            await transform
                .DOPath(path, duration, PathType.Linear, PathMode.TopDown2D)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    SetMovingState(false);
                    ResetFace();
                    DecrementSortingOrder();
                    onComplete?.Invoke();
                });
        }

        public async UniTask AttackAsync(Vector3 opponentPosition, bool isRangeAttack)
        {
            Face(transform.position, opponentPosition);

            float yDiff = opponentPosition.y - transform.position.y;
            const float eps = 0.01f;

            if (isRangeAttack)
            {
                PlayRangeAttackAnimation();
            }
            else
            {
                if (Mathf.Abs(yDiff) < eps)
                {
                    PlayFrontMeleeAttackAnimation();
                }
                else if (yDiff > 0)
                {
                    PlayTopMeleeAttackAnimation();
                }
                else
                {
                    PlayBottomMeleeAttackAnimation();
                }
            }

            await WaitEndAnimationAsync();
            ResetFace();
        }

        public async UniTask TakeDamageAsync(Vector3 opponentPosition)
        {
            Face(transform.position, opponentPosition);
            SetCount(Data.Count);
            PlayTakeDamageAnimation();

            await WaitEndAnimationAsync();
            ResetFace();
        }

        public async UniTask DeathAsync(Vector3 opponentPosition)
        {
            Face(transform.position, opponentPosition);
            PlayDeathAnimation();
            HideCount();
            DecrementSortingOrder();
            _collider2D.enabled = false;

            await WaitEndAnimationAsync();
            ResetFace();
        }

        private void SetMovingState(bool isMoving) => _animator.SetBool(IsMovingKey, isMoving);
        private void PlayFrontMeleeAttackAnimation() => _animator.Play(FrontMeleeAttackAnimationName);
        private void PlayTopMeleeAttackAnimation() => _animator.Play(TopMeleeAttackAnimationName);
        private void PlayBottomMeleeAttackAnimation() => _animator.Play(BottomMeleeAttackAnimationName);
        private void PlayRangeAttackAnimation() => _animator.Play(RangeAttackAnimationName);
        private void PlayTakeDamageAnimation() => _animator.Play(TakeDamageAnimationName);
        private void PlayDeathAnimation() => _animator.Play(DeathAnimationName);

        private void IncrementSortingOrder() => _sprite.sortingOrder++;
        private void DecrementSortingOrder() => _sprite.sortingOrder--;

        private void SetCountData()
        {
            if (Data.OwnerId.HasValue)
            {
                var color = _playerColorsService.GetRgb24(Data.OwnerId.Value).ToColor();
                _countImage.color = color;
            }
            _countText.text = Data.Count.ToString();
        }

        private void SetFlip()
        {
            _defaultFlip = Side == PlayerSide.Right;
            _sprite.flipX = _defaultFlip;
        }

        private async UniTask WaitEndAnimationAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitUntil(() => _animator == null || _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f, cancellationToken: token);
        }

        private void Face(Vector3 fromPos, Vector3 toPos)
        {
            var deltaX = toPos.x - fromPos.x;
            var shouldFlip = (Side == PlayerSide.Right && deltaX > 0) || (Side == PlayerSide.Left && deltaX < 0);
            _sprite.flipX = shouldFlip ? !_defaultFlip : _defaultFlip;
        }
        private void ResetFace() => _sprite.flipX = _defaultFlip;

        private void SetCount(int count) => _countText.text = count.ToString();
        private void HideCount()
        {
            _countContainer.gameObject.SetActive(false);
            _countImage.gameObject.SetActive(false);
            _countText.gameObject.SetActive(false);
        }

        private float CalculateMovementDuration(Vector3[] path)
        {
            float totalDistance = 0f;
            for (int i = 1; i < path.Length; i++)
            {
                totalDistance += Vector3.Distance(path[i - 1], path[i]);
            }
            return totalDistance / MoveSpeed;
        }
    }
}
