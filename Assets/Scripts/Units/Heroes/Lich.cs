using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.Attack.RangeAttack;
using Assets.Scripts.Actions.Move;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Units.Heroes
{
    public class Lich : BaseUnit
    {
        private IMove _move;
        private IMeleeAttack _meleeAttack;
        private IRangeAttack _rangeAttack;

        protected override void Start()
        {
            _move = new DefaultMove();
            _meleeAttack = new DefaultMeleeAttack();
            _rangeAttack = new DefaultRangeAttack();
            UnitMorale = 0;
            UnitRange = 7;
        }

        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            await _move.Move(attacker, targetTile);
            await _meleeAttack.MeleeAttack(attacker, defender);
            TurnManager.Instance.EndTurn(this);
        }

        public override async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
        {
            await _rangeAttack.RangeAttack(attacker, defender);
            TurnManager.Instance.EndTurn(this);
        }
    }
}
