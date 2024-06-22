using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Actions.Attack.MeleeAttack
{
    public class DefaultMeleeAttack : IMeleeAttack
    {
        public async void MeleeAttack(Tile myTile, Tile enemyTile)
        {
            var enemy = enemyTile.OccupiedUnit;
            var enemyTileCoordinates = GridManager.Instance.GetTileCoordinate(enemyTile);

            var myUnit = myTile.OccupiedUnit;
            var myUnitTileCoordinates = GridManager.Instance.GetTileCoordinate(myTile);

            myUnit.isBusy = true;

            Tile.Instance.DeleteHighlight();
            if (Math.Abs(enemyTileCoordinates.x - myUnitTileCoordinates.x) <= 1 && Math.Abs(enemyTileCoordinates.y - myUnitTileCoordinates.y) <= 1)
            {
                await UnitManager.Instance.Attack(myUnit, enemy, true, false);

                if (GameManager.Instance.GameState == GameState.HeroesTurn)
                {
                    UnitManager.Instance.SetSelectedHero(null);
                }
                UnitManager.Instance.UpdateATB();
            }

            myUnit.isBusy = false;
        }
    }
}
