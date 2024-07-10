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
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Units.Heroes
{
    public class Dragon : BaseUnit
    {
        private IMove _move;
        private IMeleeAttack _meleeAttack;

        protected override void Start()
        {
            _move = new DefaultMove();
            _meleeAttack = new DefaultMeleeAttack();
        }

        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            await _move.Move(attacker, targetTile);
            await _meleeAttack.MeleeAttack(attacker, defender);
            if (attacker.Faction == GameManager.Instance.CurrentFaction)
            {
                TurnManager.Instance.EndTurn(this);
            }
        }
    }
}
