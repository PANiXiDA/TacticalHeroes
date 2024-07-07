using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enumeration;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Actions.Move;
using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.Attack.RangeAttack;
using Assets.Scripts.Actions.TakeDamage;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Managers;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    [HideInInspector]
    public Tile OccupiedTile;
    public Faction Faction;

    public int UnitAttack;
    public int UnitDefence;
    public int UnitHealth;
    public int UnitMinDamage;
    public int UnitMaxDamage;
    public double UnitInitiative;
    public int UnitSpeed;

    [HideInInspector]
    public int? UnitRange;
    [HideInInspector]
    public int? UnitArrows;

    [HideInInspector]
    public int UnitMorale;
    [HideInInspector]
    public int UnitLuck;

    [HideInInspector]
    public int UnitFailedMorale;
    [HideInInspector]
    public int UnitSuccessfulMorale;
    [HideInInspector]
    public int UnitFailedLuck;
    [HideInInspector]
    public int UnitSuccessfulLuck;

    public List<Abilities> abilities;
    public Animator animator;

    [HideInInspector]
    public bool UnitResponse;
    [HideInInspector]
    public double UnitATB;
    [HideInInspector]
    public double UnitTime;

    [HideInInspector]
    public bool isBusy;

    private IMove _move;
    private IMeleeAttack _meleeAttack;
    private IRangeAttack _rangeAttack;
    private ITakeDamage _takeDamage;

    protected virtual void Awake()
    {
        _move = new DefaultMove();
        _meleeAttack = new DefaultMeleeAttack();
        _rangeAttack = new DefaultRangeAttack();
        _takeDamage = new DefaultTakeDamage();

        UnitMorale = 1;
        UnitLuck = 0;
        UnitFailedMorale = UnitSuccessfulMorale = UnitFailedLuck = UnitSuccessfulLuck = 0;
    }

    protected virtual void Start()
    {
        UnitInitializationManager.Instance.SetArcherParameters(this);
        UnitInitializationManager.Instance.InitializeATB(this);
    }

    public virtual async UniTask Move(BaseUnit unit, Tile targetTile)
    {
        await _move.Move(unit, targetTile);
        TurnManager.Instance.EndTurn(this);
    }

    public virtual async UniTask MeleeAttack(BaseUnit attacker, BaseUnit defender, Tile targetTile)
    {
        await _move.Move(attacker, targetTile);
        await _meleeAttack.MeleeAttack(attacker, defender);
        if (attacker.Faction == GameManager.Instance.CurrentFaction)
        {
            TurnManager.Instance.EndTurn(this);
        }
    }

    public virtual async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
    {
        await _rangeAttack.RangeAttack(attacker, defender);
        if (attacker.Faction == GameManager.Instance.CurrentFaction)
        {
            TurnManager.Instance.EndTurn(this);
        }
    }

    public virtual void TakeMeleeDamage(BaseUnit attacker, BaseUnit defender)
    {
        _takeDamage.TakeMeleeDamage(attacker, defender);
    }
    public virtual void TakeRangeDamage(BaseUnit attacker, BaseUnit defender)
    {
        _takeDamage.TakeRangeDamage(attacker, defender);
    }

    public virtual void Death()
    {
        animator.Play("Death");
        OccupiedTile.OccupiedUnit = null;

        GetComponent<SpriteRenderer>().sortingOrder = 0;

        SpawnManager.Instance.RemoveUnit(this);
    }
}
