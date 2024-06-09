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
    public void ShowOrientationOfAttack(Tile tile)
    {
        var swordsList = Resources.LoadAll<GameObject>("Swords").ToList();
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var enemyPos = new Vector3(GridManager.Instance.GetTileCoordinate(tile).x, GridManager.Instance.GetTileCoordinate(tile).y);

        float angle = Mathf.Atan2(cursorPos.y - enemyPos.y, cursorPos.x - enemyPos.x) * Mathf.Rad2Deg;
        float normalizedAngle = (angle + 360) % 360;

        Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(UnitManager.Instance.SelectedHero);
        if (UnitManager.Instance.SelectedHero != null)
        {
            if (!UnitManager.Instance.SelectedHero.abilities.Contains(Abilities.Archer))
            {
                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y + 1)) != null)
                {
                    if (normalizedAngle >= 67.5 && normalizedAngle < 112.5 && GameObject.Find("Bottom(Clone)") is null
                        && (tilesForMove.ContainsKey(new Vector2(enemyPos.x, enemyPos.y + 1))
                        || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y + 1)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "Bottom");
                        Instantiate(sword, new Vector3(enemyPos.x, enemyPos.y + (float)0.75), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }
                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y + 1)) != null)
                {
                    if (normalizedAngle >= 22.5 && normalizedAngle < 67.5 && GameObject.Find("BottomLeft(Clone)") is null
                    && (tilesForMove.ContainsKey(new Vector2(enemyPos.x + 1, enemyPos.y + 1))
                    || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y + 1)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "BottomLeft");
                        Instantiate(sword, new Vector3(enemyPos.x + (float)0.75, enemyPos.y + (float)0.75), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }

                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y)) != null)
                {
                    if ((normalizedAngle >= 0 && normalizedAngle < 22.5 || normalizedAngle >= 337.5 && normalizedAngle <= 360) &&
                    GameObject.Find("Left(Clone)") is null && (tilesForMove.ContainsKey(new Vector2(enemyPos.x + 1, enemyPos.y))
                    || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "Left");
                        Instantiate(sword, new Vector3(enemyPos.x + (float)0.75, enemyPos.y), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }

                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y - 1)) != null)
                {
                    if (normalizedAngle >= 292.5 && normalizedAngle < 337.5 && GameObject.Find("TopLeft(Clone)") is null
                    && (tilesForMove.ContainsKey(new Vector2(enemyPos.x + 1, enemyPos.y - 1))
                    || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y - 1)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "TopLeft");
                        Instantiate(sword, new Vector3(enemyPos.x + (float)0.75, enemyPos.y - (float)0.75), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }
                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y - 1)) != null)
                {
                    if (normalizedAngle >= 247.5 && normalizedAngle < 292.5 && GameObject.Find("Top(Clone)") is null
                    && (tilesForMove.ContainsKey(new Vector2(enemyPos.x, enemyPos.y - 1))
                    || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y - 1)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "Top");
                        Instantiate(sword, new Vector3(enemyPos.x, enemyPos.y - (float)0.75), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }
                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y - 1)) != null)
                {
                    if (normalizedAngle >= 202.5 && normalizedAngle < 247.5 && GameObject.Find("TopRight(Clone)") is null
                    && (tilesForMove.ContainsKey(new Vector2(enemyPos.x - 1, enemyPos.y - 1))
                    || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y - 1)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "TopRight");
                        Instantiate(sword, new Vector3(enemyPos.x - (float)0.75, enemyPos.y - (float)0.75), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }
                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y)) != null)
                {
                    if (normalizedAngle >= 157.5 && normalizedAngle < 202.5 && GameObject.Find("Right(Clone)") is null
                    && (tilesForMove.ContainsKey(new Vector2(enemyPos.x - 1, enemyPos.y))
                    || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "Right");
                        Instantiate(sword, new Vector3(enemyPos.x - (float)0.75, enemyPos.y), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }
                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y + 1)) != null)
                {
                    if (normalizedAngle >= 112.5 && normalizedAngle < 157.5 && GameObject.Find("BottomRight(Clone)") is null
                    && (tilesForMove.ContainsKey(new Vector2(enemyPos.x - 1, enemyPos.y + 1))
                    || GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y + 1)).OccupiedUnit == UnitManager.Instance.SelectedHero))
                    {
                        DeleteSwords();
                        sword = swordsList.Find(e => e.name == "BottomRight");
                        Instantiate(sword, new Vector3(enemyPos.x - (float)0.75, enemyPos.y + (float)0.75), Quaternion.identity);
                        Tile.Instance.DeleteHighlightForAttack();
                    }
                }
            }
            else
            {
                var arrowsList = Resources.LoadAll<GameObject>("Arrows").ToList();
                int distance = (int)Math.Ceiling(Math.Sqrt(Math.Pow(enemyPos.x - UnitManager.Instance.SelectedHero.OccupiedTile.Position.x, 2) +
                    Math.Pow(enemyPos.y - UnitManager.Instance.SelectedHero.OccupiedTile.Position.y, 2)));

                if (GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y)) != null)
                {
                    if (GameObject.Find("Arrow(Clone)") is null && (UnitManager.Instance.SelectedHero.UnitRange >= distance))
                    {
                        DeleteArrows();
                        var arrow = arrowsList.Find(e => e.name == "Arrow");
                        Instantiate(arrow, new Vector3(enemyPos.x - (float)0.75, enemyPos.y), Quaternion.identity);
                    }
                    else if (GameObject.Find("BreakArrow(Clone)") is null && (UnitManager.Instance.SelectedHero.UnitRange < distance))
                    {
                        DeleteArrows();
                        var arrow = arrowsList.Find(e => e.name == "BreakArrow");
                        Instantiate(arrow, new Vector3(enemyPos.x - (float)0.85, enemyPos.y), Quaternion.identity);
                    }
                }
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
