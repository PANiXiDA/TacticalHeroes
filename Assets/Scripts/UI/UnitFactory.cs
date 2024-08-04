using Assets.Scripts.Enumerations;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UnitFactory : MonoBehaviour
    {
        public static UnitFactory Instance;
        void Awake()
        {
            Instance = this;
        }

        public static GameObject CreateOrUpdateSquare(string name, Transform parent, Vector3 localPosition, Vector2 localScale, Color color, string spriteName, int sortingLayerID)
        {
            Transform existingSquare = parent.Find(name);
            GameObject square;

            if (existingSquare != null)
            {
                square = existingSquare.gameObject;
            }
            else
            {
                square = new GameObject(name);
                square.transform.SetParent(parent, true);
                square.AddComponent<SpriteRenderer>();
            }

            square.transform.localPosition = localPosition;
            square.transform.localScale = localScale;
            SpriteRenderer spriteRenderer = square.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>(spriteName);
            spriteRenderer.color = color;
            spriteRenderer.sortingLayerID = sortingLayerID;

            return square;
        }

        public static GameObject CreateOrUpdateUnitCountText(string name, Transform parent, int sortingLayerID, int fontSize, Vector3 localPosition, Vector2 sizeDelta, Vector2 localScale, string text)
        {
            Transform existingUnitCount = parent.Find(name);
            GameObject unitCount;

            if (existingUnitCount != null)
            {
                unitCount = existingUnitCount.gameObject;
            }
            else
            {
                unitCount = new GameObject(name);
                unitCount.transform.SetParent(parent, true);
                unitCount.AddComponent<TextMeshPro>();
            }

            TextMeshPro unitCountTextMeshPro = unitCount.GetComponent<TextMeshPro>();
            unitCountTextMeshPro.sortingLayerID = sortingLayerID;
            unitCountTextMeshPro.sortingOrder = 1;
            unitCountTextMeshPro.fontSize = fontSize;
            unitCountTextMeshPro.rectTransform.localPosition = localPosition;
            unitCountTextMeshPro.rectTransform.sizeDelta = sizeDelta;
            unitCountTextMeshPro.rectTransform.localScale = localScale;
            unitCountTextMeshPro.alignment = TextAlignmentOptions.Center;
            unitCountTextMeshPro.text = text;

            return unitCount;
        }

        public void CreateOrUpdateUnitVisuals(BaseUnit unit)
        {
            var menuLayerId = SortingLayer.NameToID("Menu");
            var countNumbersInCountUnits = unit.UnitCount.ToSafeString().Count();
            int fontSize = 5;
            float widthSquare = 0.2f + 0.1f * countNumbersInCountUnits;
            Color color = unit.Side == Side.Player ? Color.red : Color.blue;
            float widthText = countNumbersInCountUnits < 3 ? countNumbersInCountUnits == 1 ? 2.5f : 2f : 1.5f;

            GameObject square = CreateOrUpdateSquare("Square", unit.transform, new Vector3(0.23f, -0.3f, 0),
                                             new Vector2(widthSquare, 0.3f), color, "Square", menuLayerId);

            GameObject unitCount = CreateOrUpdateUnitCountText("UnitCount", square.transform, menuLayerId, fontSize, Vector3.zero,
                                                       new Vector2(0.28f * countNumbersInCountUnits, 0.45f), new Vector2(widthText, 3f), unit.UnitCount.ToString());
        }

        public void CreateDamageVisuals(BaseUnit attacker, BaseUnit defender, int damage, int countDeath)
        {
            var menuLayerId = SortingLayer.NameToID("Menu");

            GameObject damageVisual = new GameObject("DamageVisual");
            damageVisual.AddComponent<TextMeshPro>();

            TextMeshPro damageVisualTextMeshPro = damageVisual.GetComponent<TextMeshPro>();
            damageVisualTextMeshPro.sortingLayerID = menuLayerId;
            damageVisualTextMeshPro.sortingOrder = 1;
            damageVisualTextMeshPro.fontSize = 6;
            damageVisualTextMeshPro.rectTransform.transform.position = defender.transform.position;
            damageVisualTextMeshPro.rectTransform.sizeDelta = new Vector2(3, 1);
            damageVisualTextMeshPro.alignment = GridManager.Instance.GetTileCoordinate(attacker.OccupiedTile).x < GridManager.Instance.GetTileCoordinate(defender.OccupiedTile).x ?
                TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            damageVisualTextMeshPro.text = $"<sprite=\"Damage\" index=0>{damage}\n " +
                $"{(countDeath > 0 ? $"<sprite=\"Death\" index=0>{countDeath}" : "")}";

            FadeOutAndDestroy(damageVisualTextMeshPro).Forget();
        }

        private async UniTaskVoid FadeOutAndDestroy(TextMeshPro textMeshPro)
        {
            Color originalColor = textMeshPro.color;
            Vector3 originalPosition = textMeshPro.transform.position;
            float elapsedTime = 0f;
            float duration = 5f;
            float riseSpeed = 6f; // Увеличенная скорость подъема текста

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                textMeshPro.transform.position = originalPosition + Vector3.up * (elapsedTime / duration) * riseSpeed;

                await UniTask.Yield();
            }

            Destroy(textMeshPro.gameObject);
        }

        public async UniTaskVoid CreateMoraleEffect(BaseUnit unit)
        {
            var menuLayerId = SortingLayer.NameToID("Menu");
            var unitPosition = unit.transform.position;

            GameObject morale = new GameObject("Morale");
            morale.transform.position = new Vector3(unitPosition.x, unitPosition.y + 1, unitPosition.z);

            SpriteRenderer spriteRenderer = morale.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerID = menuLayerId;
            spriteRenderer.sortingOrder = 2;

            Animator animator = morale.AddComponent<Animator>();
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("Animations/BattleEffects/Morale/Morale");
            animator.runtimeAnimatorController = controller;

            animator.Play("Morale");
            await UniTask.Delay(500);

            Destroy(animator.gameObject);
        }

        public async UniTaskVoid CreateLuckEffect(BaseUnit unit)
        {
            var menuLayerId = SortingLayer.NameToID("Menu");
            var unitPosition = unit.transform.position;

            GameObject morale = new GameObject("Luck");
            morale.transform.position = new Vector3(unitPosition.x, unitPosition.y + 1, unitPosition.z);

            SpriteRenderer spriteRenderer = morale.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerID = menuLayerId;
            spriteRenderer.sortingOrder = 2;

            Animator animator = morale.AddComponent<Animator>();
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("Animations/BattleEffects/Luck/Luck");
            animator.runtimeAnimatorController = controller;

            animator.Play("Luck");
            await UniTask.Delay(500);

            Destroy(animator.gameObject);
        }
    }
}
