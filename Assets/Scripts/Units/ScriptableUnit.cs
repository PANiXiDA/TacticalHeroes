using Assets.Scripts.Enumerations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scritpable Unit")]

public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
}
