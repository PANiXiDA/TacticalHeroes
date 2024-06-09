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

    [SerializeField] private GameObject _tileInfoPanel, _unitInfoPanel, _chatPanel, _winPanel, _losePanel;
    [SerializeField] private RectTransform _ATBIcons;

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
        _chatPanel.GetComponentInChildren<Text>().text += hero.UnitName + 
            " нанес " + damage.ToString() + " урона по " + enemy.UnitName + "\n";
    }
    public void ShowUnitsPortraits()
    {
        foreach (Transform child in _ATBIcons)
        {
            Destroy(child.gameObject);
        }

        foreach (var ATBunit in UnitManager.Instance.ATB)
        {
            string unitName = ATBunit.name.Replace("(Clone)", "");

            GameObject portrait = new GameObject(unitName);
            portrait.transform.SetParent(_ATBIcons, false);

            Image image = portrait.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>($"Icons/{unitName}");

            GameObject contour = new GameObject("Countour");
            contour.transform.SetParent(portrait.transform, false);
            contour.transform.localPosition = new Vector3(190, -180, 0);
            contour.transform.localScale = new Vector3(180, 170, 1);

            SpriteRenderer spriteRenderer = contour.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Countour");
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.color = ATBunit.Faction == Faction.Hero ? Color.red : Color.blue;
        }
    }
    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            HideTileInfoPanels();
            return;
        }

        UpdateTileInfoPanel(tile);

        if (tile.OccupiedUnit != null)
        {
            UpdateUnitInfoPanel(tile.OccupiedUnit);
        }
        else
        {
            _unitInfoPanel.SetActive(false);
        }
    }
    private void HideTileInfoPanels()
    {
        _tileInfoPanel.SetActive(false);
        _unitInfoPanel.SetActive(false);
    }
    private void UpdateTileInfoPanel(Tile tile)
    {
        var tileCoordinate = GridManager.Instance.GetTileCoordinate(tile);
        _tileInfoPanel.GetComponentInChildren<Text>().text = $"x = {tileCoordinate.x}\ny = {tileCoordinate.y}";
        _tileInfoPanel.SetActive(true);
    }
    private void UpdateUnitInfoPanel(BaseUnit unit)
    {
        string abilitiesText = string.Join(". ", unit.abilities.Select(ability => EnumHelper.GetDescription(ability)));

        SetUnitInfoText("UnitName", unit.UnitName);
        SetUnitInfoText("AttackValue", unit.UnitAttack.ToString());
        SetUnitInfoText("DefenceValue", unit.UnitDefence.ToString());
        SetUnitInfoText("HealthValue", unit.UnitHealth.ToString());
        SetUnitInfoText("ArrowsValue", unit.UnitArrows != null ? unit.UnitArrows.ToString() : "-");
        SetUnitInfoText("RangeValue", unit.UnitRange != null ? unit.UnitRange.ToString() : "-");
        SetUnitInfoText("DamageValue", $"{unit.UnitMinDamage} - {unit.UnitMaxDamage}");
        SetUnitInfoText("InitiativeValue", unit.UnitInitiative.ToString().Replace(',', '.'));
        SetUnitInfoText("MoraleValue", unit.UnitMorale.ToString());
        SetUnitInfoText("LuckValue", unit.UnitLuck.ToString());
        SetUnitInfoText("Abilities", abilitiesText);

        _unitInfoPanel.SetActive(true);
    }
    private void SetUnitInfoText(string parameterName, string textValue)
    {
        _unitInfoPanel.GetComponentsInChildren<TextMeshProUGUI>(true)
            .Where(item => item.name == parameterName)
            .FirstOrDefault()
            .text = textValue;
    }
    public void WinPanel()
    {
        _winPanel.SetActive(true);
    }
    public void LosePanel()
    {
        _losePanel.SetActive(true);
    }
}
