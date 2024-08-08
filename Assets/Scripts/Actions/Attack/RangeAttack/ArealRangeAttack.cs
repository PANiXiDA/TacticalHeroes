using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Actions.Attack.RangeAttack
{
    public class ArealRangeAttack : MonoBehaviour, IRangeAttack
    {
        private IDamage _damageCalculator;

        public void Initialize(IDamage damageCalculator)
        {
            _damageCalculator = damageCalculator;
        }

        public async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
        {
            Tile.Instance.DeleteHighlight();
            attacker.isBusy = true;

            attacker.UnitArrows -= 1;
            attacker.animator.Play("RangeAttack");

            bool isLuck = UnitManager.Instance.Luck(attacker);
            _damageCalculator.isLuck = isLuck;

            CreateCloudEffect(defender).Forget();

            List<BaseUnit> defenders = GetArealUnits(defender);
            foreach (var defenderForAttack in defenders)
            {
                if (defenderForAttack == defender)
                {
                    await defenderForAttack.TakeRangeDamage(attacker, defenderForAttack, _damageCalculator);
                    bool death = UnitManager.Instance.IsDead(defenderForAttack);
                    if (death)
                    {
                        await defenderForAttack.Death();
                    }
                }
                else
                {
                    _ = defenderForAttack.TakeRangeDamage(attacker, defenderForAttack, _damageCalculator);
                    bool death = UnitManager.Instance.IsDead(defenderForAttack);
                    if (death)
                    {
                        _ = defenderForAttack.Death();
                    }
                }
            }

            await UniTask.Delay(1000);
            attacker.isBusy = false;
        }

        private List<BaseUnit> GetArealUnits(BaseUnit defender)
        {
            List<BaseUnit> defenders = new List<BaseUnit>();

            var directions = new Vector2Int[] {
                new Vector2Int(1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 1)
            };

            Vector2 defenderPosition = GridManager.Instance.GetTileCoordinate(defender.OccupiedTile);

            foreach (var direction in directions)
            {
                Tile tile = GridManager.Instance.GetTileAtPosition(defenderPosition + direction);
                if (tile != null)
                {
                    if (tile.OccupiedUnit != null)
                    {
                        defenders.Add(tile.OccupiedUnit);
                    }
                }
            }

            defenders.Add(defender);

            return defenders;
        }

        private async UniTaskVoid CreateCloudEffect(BaseUnit unit)
        {
            var menuLayerId = SortingLayer.NameToID("Menu");
            var unitPosition = unit.transform.position;

            GameObject cloud = new GameObject("Cloud");
            cloud.transform.position = new Vector3(unitPosition.x, unitPosition.y, unitPosition.z);
            cloud.transform.localScale = new Vector3(2, 2, 2);

            SpriteRenderer spriteRenderer = cloud.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerID = menuLayerId;
            spriteRenderer.sortingOrder = 2;

            Animator animator = cloud.AddComponent<Animator>();
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("Animations/Lich/Cloud");
            animator.runtimeAnimatorController = controller;

            animator.Play("Cloud");
            await UniTask.Delay(1000);

            Destroy(animator.gameObject);
        }
    }
}
