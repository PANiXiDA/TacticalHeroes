﻿using Assets.Scripts.Units.Enemies;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Citadel
{
    public class AncientBehemoth : Bear
    {
        protected override void Start()
        {
            base.Start();
            PercentIgnoreDefence = 0.7;
        }

        public override async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
        {
            var defence = defender.UnitDefence;
            defender.UnitDefence = (int)(defender.UnitDefence * (1 - PercentIgnoreDefence));
            await base.MeleeAttack(attacker, defender, targetTile);
            defender.UnitDefence = defence;
        }
    }
}
