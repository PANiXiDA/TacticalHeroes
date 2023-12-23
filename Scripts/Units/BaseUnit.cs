using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int UnitAttack;
    public int UnitDefence;
    public int UnitHealth;
    public int UnitMinDamage;
    public int UnitMaxDamage;
    public int UnitInitiative;
    public int UnitSpeed;
    public Animator animator;

}
