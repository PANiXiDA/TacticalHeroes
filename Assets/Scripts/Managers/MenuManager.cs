using Assets.Scripts.Enumeration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Helpers;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _tileObject, _tileUnitObject, _consoleObject, sword, _winObject, _loseObject;
    [SerializeField] private RectTransform _contentRectTransform;

    private void Awake()
    {
        Instance = this;
    }
    public void ShowOrientationOfAttack(Tile tile)
    {
        var enemyPos = GridManager.Instance.GetTileCoordinate(tile);
        var cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float normalizedAngle = Mathf.Atan2(cursorPos.y - enemyPos.y, cursorPos.x - enemyPos.x) * Mathf.Rad2Deg;
        normalizedAngle = (normalizedAngle + 360) % 360;

        var selectedHero = UnitManager.Instance.SelectedHero;
        var tilesForMove = UnitManager.Instance.GetTilesForMove(selectedHero);

        if (selectedHero != null)
        {
            if (!selectedHero.abilities.Contains(Abilities.Archer))
            {
                ShowSwordAttack(enemyPos, normalizedAngle, tilesForMove);
            }
            else
            {
                ShowArrowAttack(enemyPos, selectedHero);
            }
        }
    }
    private void ShowSwordAttack(Vector2 enemyPos, float normalizedAngle, Dictionary<Vector2, Tile> tilesForMove)
    {
        var swordsList = Resources.LoadAll<GameObject>("Swords").ToList();
        var swordPositions = new Dictionary<string, (Vector2 position, float angleRangeStart, float angleRangeEnd)>
        {
            {"Bottom", (new Vector2(enemyPos.x, enemyPos.y + 0.75f), 67.5f, 112.5f)},
            {"BottomLeft", (new Vector2(enemyPos.x + 0.75f, enemyPos.y + 0.75f), 22.5f, 67.5f)},
            {"Left", (new Vector2(enemyPos.x + 0.75f, enemyPos.y), 0f, 22.5f)},
            {"TopLeft", (new Vector2(enemyPos.x + 0.75f, enemyPos.y - 0.75f), 292.5f, 337.5f)},
            {"Top", (new Vector2(enemyPos.x, enemyPos.y - 0.75f), 247.5f, 292.5f)},
            {"TopRight", (new Vector2(enemyPos.x - 0.75f, enemyPos.y - 0.75f), 202.5f, 247.5f)},
            {"Right", (new Vector2(enemyPos.x - 0.75f, enemyPos.y), 157.5f, 202.5f)},
            {"BottomRight", (new Vector2(enemyPos.x - 0.75f, enemyPos.y + 0.75f), 112.5f, 157.5f)},
        };

        foreach (var (swordName, (position, angleStart, angleEnd)) in swordPositions)
        {
            var tilePos = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            var tile = GridManager.Instance.GetTileAtPosition(tilePos);

            if (tile != null &&
                normalizedAngle >= angleStart && normalizedAngle < angleEnd &&
                GameObject.Find($"{swordName}(Clone)") is null &&
                (tilesForMove.ContainsKey(tilePos) || tile.OccupiedUnit == UnitManager.Instance.SelectedHero))
            {
                DeleteSwords();
                var sword = swordsList.Find(e => e.name == swordName);
                Instantiate(sword, position, Quaternion.identity);
            }
        }
    }
    private void ShowArrowAttack(Vector2 enemyPos, BaseHero selectedHero)
    {
        var arrowsList = Resources.LoadAll<GameObject>("Arrows").ToList();

        var distance = (int)Math.Ceiling(Math.Sqrt(Math.Pow(enemyPos.x - selectedHero.OccupiedTile.Position.x, 2) +
            Math.Pow(enemyPos.y - selectedHero.OccupiedTile.Position.y, 2)));

        var tilePos = new Vector2(enemyPos.x - 1, enemyPos.y);
        var tile = GridManager.Instance.GetTileAtPosition(tilePos);

        if (tile != null && GameObject.Find("Arrow(Clone)") is null && selectedHero.UnitRange >= distance)
        {
            DeleteArrows();
            var arrow = arrowsList.Find(e => e.name == "Arrow");
            Instantiate(arrow, new Vector3(tilePos.x + 0.25f, tilePos.y), Quaternion.identity);
        }
        else if (tile != null && GameObject.Find("BreakArrow(Clone)") is null && selectedHero.UnitRange < distance)
        {
            DeleteArrows();
            var arrow = arrowsList.Find(e => e.name == "BreakArrow");
            Instantiate(arrow, new Vector3(tilePos.x + 0.15f, tilePos.y), Quaternion.identity);
        }
    }

    public void DeleteSwords()
    {
        var swordsList = Resources.LoadAll<GameObject>("Swords").ToList();
        foreach (var sword in swordsList)
        {
            if (GameObject.Find(sword.name + "(Clone)") is not null)
            {
                Destroy(GameObject.Find(sword.name + "(Clone)"));
            }
        }
    }

    public void DeleteArrows()
    {
        var arrowsList = Resources.LoadAll<GameObject>("Arrows").ToList();
        foreach (var arrow in arrowsList)
        {
            if (GameObject.Find(arrow.name + "(Clone)") is not null)
            {
                Destroy(GameObject.Find(arrow.name + "(Clone)"));
            }
        }
    }

    public void ShowDamage(BaseUnit hero, BaseUnit enemy, int damage)
    {
        _consoleObject.GetComponentInChildren<Text>().text += hero.UnitName + 
            " нанес " + damage.ToString() + " урона по " + enemy.UnitName + "\n";
    }
    public void ShowUnitsPortraits()
    {
        for (int i = _contentRectTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(_contentRectTransform.GetChild(i).gameObject);
        }
        foreach (var ATBunit in UnitManager.Instance.ATB)
        {
            string unitName = ATBunit.name.Replace("(Clone)", "");
            GameObject newImage = new GameObject(unitName);
            Image image = newImage.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("Icons/" + unitName);
            newImage.transform.SetParent(_contentRectTransform, false);

            GameObject countour = new GameObject("Countour");
            countour.transform.SetParent(newImage.transform, false);


            countour.transform.localPosition = new Vector3(190, -180, 0);
            countour.transform.localScale = new Vector3(180, 170, 1);

            SpriteRenderer spriteRenderer = countour.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Countour");
            spriteRenderer.sortingOrder = 1;

            if (ATBunit.Faction == Faction.Hero)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.blue;
            }
        }
    }
    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }
        _tileObject.GetComponentInChildren<Text>().text = "x = " + GridManager.Instance.GetTileCoordinate(tile).x + "\n" +
            "y = " + GridManager.Instance.GetTileCoordinate(tile).y;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            string abilitiesText = "";
            foreach (var ability in tile.OccupiedUnit.abilities)
            {
                abilitiesText += EnumHelper.GetDescription(ability) + ". ";
            }
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "UnitName").FirstOrDefault().text = tile.OccupiedUnit.UnitName;
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "AttackValue").FirstOrDefault().text = $"{tile.OccupiedUnit.UnitAttack}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "DefenceValue").FirstOrDefault().text = $"{tile.OccupiedUnit.UnitDefence}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "HealthValue").FirstOrDefault().text = $"{tile.OccupiedUnit.UnitHealth}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "ArrowsValue").FirstOrDefault().text = $"{(tile.OccupiedUnit.UnitArrows != null ? tile.OccupiedUnit.UnitArrows : "-")}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "RangeValue").FirstOrDefault().text = $"{(tile.OccupiedUnit.UnitRange != null ? tile.OccupiedUnit.UnitRange : "-")}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "DamageValue").FirstOrDefault().text = $"{tile.OccupiedUnit.UnitMinDamage} - {tile.OccupiedUnit.UnitMaxDamage}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "InitiativeValue").FirstOrDefault().text = $"{tile.OccupiedUnit.UnitInitiative.ToString().Replace(',', '.')}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "MoraleValue").FirstOrDefault().text = $"{tile.OccupiedUnit.UnitMorale}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "LuckValue").FirstOrDefault().text = $"{tile.OccupiedUnit.UnitLuck}";
            _tileUnitObject.GetComponentsInChildren<TextMeshProUGUI>(true)
                .Where(item => item.name == "Abilities").FirstOrDefault().text = abilitiesText;

            _tileUnitObject.SetActive(true);
        }
    }
    public void WinPanel()
    {
        _winObject.SetActive(true);
    }
    public void LosePanel()
    {
        _loseObject.SetActive(true);
    }
}
