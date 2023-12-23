using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject, _consoleObject;

    private void Awake()
    {
        Instance = this;
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
