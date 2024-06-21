using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public interface IMeleeAttack
    {
        public void MeleeAttack(Tile myTile, Tile enemyTile);
    }
}
