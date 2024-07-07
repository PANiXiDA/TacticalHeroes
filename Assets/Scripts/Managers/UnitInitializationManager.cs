using Assets.Scripts.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class UnitInitializationManager : MonoBehaviour
    {
        public static UnitInitializationManager Instance;
        void Awake()
        {
            Instance = this;
        }
        public void InitializeATB(BaseUnit unit)
        {
            unit.UnitATB = UnityEngine.Random.Range(0f, 15f);
            unit.UnitTime = (100 - unit.UnitATB) / unit.UnitInitiative;
        }

        public void SetArcherParameters(BaseUnit unit)
        {
            if (unit.abilities.Contains(Abilities.Archer))
            {
                unit.UnitRange = 6;
                unit.UnitArrows = 10;
            }
        }

        public void CalculateProbabilityStats(BaseUnit unit)
        {
            var probabilityLuck = Math.Pow(unit.UnitLuck / 10.0, 1 + unit.UnitSuccessfulLuck - unit.UnitFailedLuck * (unit.UnitLuck / 10.0 / (1 - unit.UnitLuck / 10.0)));
            var probabilityMorale = Math.Pow(unit.UnitMorale / 10.0, 1 + unit.UnitSuccessfulMorale - unit.UnitFailedMorale * (unit.UnitMorale / 10.0 / (1 - unit.UnitMorale / 10.0)));
        }
    }
}
