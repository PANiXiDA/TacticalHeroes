using Assets.Scripts.GameEngine.DTO.DamageCalculator;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using Assets.Scripts.Domain.GameEngine.DTO.Extensions;

using Assets.Scripts.GameEngine.Domain.Core;
using System.Collections.Generic;
using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.Domain.GameEngine.Interfaces;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;
using R3;
using System;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class AttacksService : IAttacksService
    {
        private readonly IGameObjectValidation _gameObjectValidation;
        private readonly IDamageCalculator _damageCalculator;
        private readonly IAttackCalculator _attackCalculator;
        private readonly IMovementsService _movementsService;
        private readonly IGridsService _gridsService;

        private readonly Subject<List<AttackEvent>> _attackPrepared = new();

        private readonly Dictionary<Guid, UniTaskCompletionSource> _attackSources = new();

        private readonly List<AttackEvent> _attackEventsCache = new();

        public Observable<List<AttackEvent>> OnAttackPrepared => _attackPrepared.AsObservable();


        public AttacksService(
            IGameObjectValidation gameObjectValidation,
            IDamageCalculator damageCalculator,
            IAttackCalculator attackCalculator,
            IMovementsService movementsService,
            IGridsService gridsService)
        {
            _gameObjectValidation = gameObjectValidation;
            _damageCalculator = damageCalculator;
            _attackCalculator = attackCalculator;
            _movementsService = movementsService;
            _gridsService = gridsService;
        }

        public async UniTask MeleeAttackAsync(Unit attacker, Unit defender, Tile targetTile, bool isResponseAttack = false)
        {
            if (_attackSources.ContainsKey(attacker.Id))
            {
                await _attackSources[attacker.Id].Task;
                return;
            }

            if (!isResponseAttack)
            {
                await _movementsService.MoveAsync(targetTile, attacker);
            }

            var countAttacks = isResponseAttack ? 1 : _attackCalculator.GetCountMeleeAttacks(attacker);

            for (int i = 0; i < countAttacks; i++)
            {
                double damageModifier = 1d;
                var damage = ComputeDamage(attacker, defender, damageModifier);
                var damageResult = ComputeDamageResult(defender, damage);

                UpdateDefenderStats(defender, damageResult);

                _attackEventsCache.Add(new AttackEvent(attacker, defender, damage, damageResult.DeadUnits, false));

                if (_attackCalculator.HasResponseMeleeAttack(defender, isResponseAttack))
                {
                    await MeleeAttackAsync(defender, attacker, null, true);
                    defender.HasResponseMeleeAttack = false;
                }
            }

            if (!isResponseAttack)
            {
                await AwaitAnimationsAsync(attacker.Id);
            }

            return;
        }

        public async UniTask RangeAttackAsync(GameObject attacker, Unit defender, bool isResponseAttack = false)
        {
            if (_attackSources.ContainsKey(attacker.Id))
            {
                await _attackSources[attacker.Id].Task;
                return;
            }
            if (!_gameObjectValidation.IsValidArcher(attacker))
            {
                return;
            }

            _movementsService.ClearReachableTiles();

            var countAttacks = isResponseAttack ? 1 : _attackCalculator.GetCountRangeAttacks(attacker);

            for (int i = 0; i < countAttacks; i++)
            {
                double damageModifier = 1d;
                damageModifier *= GetDistanceModifier(attacker, defender);
                var damage = ComputeDamage(attacker, defender, damageModifier);
                var damageResult = ComputeDamageResult(defender, damage);

                UpdateAttackerArrows(attacker);
                UpdateDefenderStats(defender, damageResult);

                _attackEventsCache.Add(new AttackEvent(attacker, defender, damage, damageResult.DeadUnits, true));

                if (_attackCalculator.HasResponseRangeAttack(defender, isResponseAttack) && attacker is Unit unit)
                {
                    await RangeAttackAsync(defender, unit, true);
                    defender.HasResponseMeleeAttack = false;
                }
            }

            if (!isResponseAttack)
            {
                await AwaitAnimationsAsync(attacker.Id);
            }

            return;
        }

        public void NotifyAttackCompleted(Guid attackerId)
        {
            if (_attackSources.TryGetValue(attackerId, out var task))
            {
                task.TrySetResult();
                _attackSources.Remove(attackerId);
                _attackEventsCache.Clear();
            }
        }

        private UniTask AwaitAnimationsAsync(Guid rootId)
        {
            var task = new UniTaskCompletionSource();
            _attackSources[rootId] = task;

            _attackPrepared.OnNext(new List<AttackEvent>(_attackEventsCache));
            return task.Task;
        }

        private int ComputeDamage(GameObject attacker, Unit defender, double damageModifier)
        {
            int count = 1;
            int attackerAttack = attacker.Attack;
            int attackerMinDmg = attacker.MinDamage;
            int attackerMaxDmg = attacker.MaxDamage;

            if (attacker is Unit attackerUnit)
            {
                count = attackerUnit.Count;
                attackerAttack = attackerUnit.EffectiveAttack();
                attackerMinDmg = attackerUnit.EffectiveMinDamage();
                attackerMaxDmg = attackerUnit.EffectiveMaxDamage();
            }

            var damageContext = new DamageContext(
                attackerAttack: attackerAttack,
                attackerMinDamage: attackerMinDmg,
                attackerMaxDamage: attackerMaxDmg,
                attackerCount: count,
                defenderDefence: defender.EffectiveDefence(),
                damageModifier: damageModifier);

            return _damageCalculator.CalculateDamage(damageContext);
        }

        private DamageResult ComputeDamageResult(Unit defender, int damage)
        {
            var defenderTakeDamageContext = new DefenderTakeDamageContext(
                defenderCurrentHealth: defender.CurrentHealth,
                defenderFullHealth: defender.EffectiveHealth(),
                defenderCount: defender.Count,
                attackerDamage: damage);

            return _damageCalculator.ComputeCasualties(defenderTakeDamageContext);
        }

        private double GetDistanceModifier(GameObject attacker, Unit defender)
        {
            if (attacker is not Unit unit)
            {
                return 1d;
            }

            var attackerTile = _gridsService.GetTile(attacker.Id);
            var defenderTile = _gridsService.GetTile(defender.Id);

            var distance = _gridsService.GetDistance(attackerTile, defenderTile);

            return unit.Range.Value > distance ? 1d : 0.5d;
        }

        private void UpdateDefenderStats(Unit defender, DamageResult damageResult)
        {
            defender.CurrentHealth = damageResult.RemainingHealth;
            defender.Count = damageResult.SurvivingUnits;
        }

        private void UpdateAttackerArrows(GameObject attacker)
        {
            if (attacker is Unit unit)
            {
                unit.Arrows--;
            }
        }
    }
}
