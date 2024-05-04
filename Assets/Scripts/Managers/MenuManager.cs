using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject, _consoleObject, sword;

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
    public void ShowOrientationOfAttack(Tile tile)
    {
        var swordsList = Resources.LoadAll<GameObject>("Swords").ToList();
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var enemyPos = new Vector3(GridManager.Instance.GetTileCoordinate(tile).x, GridManager.Instance.GetTileCoordinate(tile).y);

        float angle = Mathf.Atan2(cursorPos.y - enemyPos.y, cursorPos.x - enemyPos.x) * Mathf.Rad2Deg;
        float normalizedAngle = (angle + 360) % 360;

        Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(UnitManager.Instance.SelectedHero);

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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y + 1));
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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y + 1));
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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y));
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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y - 1));
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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y - 1));
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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y - 1));
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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y));
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
                //return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y + 1));
            }
        }
        //return null;
    }
    public void ShowDamage(BaseUnit hero, BaseUnit enemy, int damage)
    {
        _consoleObject.GetComponentInChildren<Text>().text += hero.UnitName + 
            " нанес " + damage.ToString() + " урона по " + enemy.UnitName + "\n";
    }
    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }
        _tileObject.GetComponentInChildren<Text>().text = "x = " + 
            GridManager.Instance.GetTileCoordinate(tile).x + " y = " + GridManager.Instance.GetTileCoordinate(tile).y;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            _tileUnitObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.UnitName + "\n" + 
                "Атака: " + tile.OccupiedUnit.UnitAttack.ToString() + "\n" +
                "Защита: " + tile.OccupiedUnit.UnitDefence.ToString() + "\n" +
                "Здоровье: " + tile.OccupiedUnit.UnitHealth.ToString() + "\n" +
                "Урон: " + tile.OccupiedUnit.UnitMinDamage.ToString() + "-" + tile.OccupiedUnit.UnitMaxDamage.ToString() + "\n" +
                "Инициатива: " + tile.OccupiedUnit.UnitInitiative.ToString() + "\n" +
                "Скорость: " + tile.OccupiedUnit.UnitSpeed.ToString() + "\n";
            //_tileUnitObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.UnitAttack.ToString();
            _tileUnitObject.SetActive(true);
        }
    }
    public void ShowSelectedHero(BaseHero hero)
    {
        if (hero == null)
        {
            _selectedHeroObject.SetActive(false);
            return;
        }
        _selectedHeroObject.GetComponentInChildren<Text>().text = hero.UnitName;
        _selectedHeroObject.SetActive(true);
    }

}
