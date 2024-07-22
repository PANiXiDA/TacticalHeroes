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
using Assets.Scripts.IActions;
using Assets.Scripts.Actions.Damage;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    [HideInInspector]
    public Tile OccupiedTile;
    public Faction Faction;

    public int UnitAttack;
    public int UnitDefence;
    public int UnitFullHealth;
    public int UnitMinDamage;
    public int UnitMaxDamage;
    public double UnitInitiative;
    public int UnitSpeed;

    public int UnitCount;

    [HideInInspector]
    public int UnitCurrentHealth;
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

    private IDamage _damageCalculator;
    private IMove _move;
    private IMeleeAttack _meleeAttack;
    private IRangeAttack _rangeAttack;
    private ITakeDamage _takeDamage;

    protected virtual void Awake()
    {
        _damageCalculator = new DefaultDamage();
        _move = new DefaultMove();
        _meleeAttack = new DefaultMeleeAttack(_damageCalculator);
        _rangeAttack = new DefaultRangeAttack(_damageCalculator);
        _takeDamage = new DefaultTakeDamage();

        UnitCurrentHealth = UnitFullHealth;
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

    public virtual async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
    {
        await _takeDamage.TakeMeleeDamage(attacker, defender, damageCalculator);
    }
    public virtual async UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
    {
        await _takeDamage.TakeRangeDamage(attacker, defender, damageCalculator);
    }

    public virtual async UniTask Death()
    {
        SpawnManager.Instance.DestroyUnitChildrenObjects(this);

        animator.Play("Death");
        await UniTask.Delay(1000);

        OccupiedTile.OccupiedUnit = null;

        MenuManager.Instance.ClearExistingIcons(this);
        SpawnManager.Instance.RemoveUnit(this);
    }
}
