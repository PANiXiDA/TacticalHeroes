using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.RangeAttack
{
    public class DefaultRangeAttack : IRangeAttack
    {
        public async void RangeAttack(Tile myTile, Tile enemyTile)
        {
            var enemyUnit = enemyTile.OccupiedUnit;
            var myUnit = myTile.OccupiedUnit;

            myUnit.isBusy = true;

            Tile.Instance.DeleteHighlight();
            await UnitManager.Instance.Attack(myUnit, enemyUnit, false, false);

            if (GameManager.Instance.GameState == GameState.HeroesTurn)
            {
                UnitManager.Instance.SetSelectedHero(null);
            }
            UnitManager.Instance.UpdateATB();

            myUnit.isBusy = false;
        }
    }
}
