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
            unit.UnitATB = Random.Range(0f, 15f);
            unit.UnitTime = (100 - unit.UnitATB) / unit.UnitInitiative;
        }

        public void SetArcherParameters(BaseUnit unit)
        {
            if (unit.abilities.Contains(UnitAbility.Archer))
            {
                unit.UnitRange = 6;
                unit.UnitArrows = 10;
            }
        }
    }
}
