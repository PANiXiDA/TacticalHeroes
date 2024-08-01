using Assets.Scripts.Enumerations;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance;

        public List<BaseUnit> allUnits = new List<BaseUnit>();
        public List<KeyValuePair<double, BaseUnit>> ATB = new List<KeyValuePair<double, BaseUnit>>();

        private void Awake()
        {
            Instance = this;
        }

        public void SetATB()
        {
            allUnits.AddRange(SpawnManager.Instance.PlayerUnits);
            allUnits.AddRange(SpawnManager.Instance.EnemyUnits);
            allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));

            while (ATB.Count < 100)
            {
                var time = allUnits.FirstOrDefault().UnitTime;
                foreach (var unit in allUnits)
                {
                    unit.UnitATB += unit.UnitInitiative * time;
                    if (unit.UnitATB >= 100)
                    {
                        unit.UnitATB -= 100;
                        ATB.Add(KeyValuePair.Create(0d, unit));
                    }
                    unit.UnitTime = (100 - unit.UnitATB) / unit.UnitInitiative;
                }
                for (int i = 0; i < ATB.Count; i++)
                {
                    ATB[i] = new KeyValuePair<double, BaseUnit>(ATB[i].Key - time, ATB[i].Value);
                }
                allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));
            }

            MenuManager.Instance.ShowUnitsPortraits();

            StartTurn(ATB.FirstOrDefault().Value);
        }

        public void UpdateATB(BaseUnit unit)
        {
            BaseUnit currentUnit = ATB.FirstOrDefault().Value;
            if (currentUnit == unit)
            {
                var time = currentUnit.UnitTime;
                ATB.RemoveAt(0);

                List<BaseUnit> newUnitsInATB = new List<BaseUnit>();

                foreach (var ATBunit in allUnits)
                {
                    for (int i = 0; i < ATB.Count; i++)
                    {
                        ATB[i] = new KeyValuePair<double, BaseUnit>(ATB[i].Key - time, ATB[i].Value);
                    }

                    ATBunit.UnitATB += ATBunit.UnitInitiative * time;
                    if (ATBunit.UnitATB >= 100)
                    {
                        ATBunit.UnitATB -= 100;
                        ATB.Add(KeyValuePair.Create(0d, ATBunit));
                        newUnitsInATB.Add(ATBunit);
                    }
                    ATBunit.UnitTime = (100 - ATBunit.UnitATB) / ATBunit.UnitInitiative;
                }
                allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));

                MenuManager.Instance.UpdatePortraits(newUnitsInATB);
            }
        }
        public void WaitOrMoraleUnit(BaseUnit unit)
        {
            BaseUnit currentUnit = ATB.FirstOrDefault().Value;
            if (currentUnit == unit)
            {
                var time = currentUnit.UnitTime;
                //ATB.RemoveAt(0);

                for (int i = 0; i < ATB.Count; i++)
                {
                    ATB[i] = new KeyValuePair<double, BaseUnit>(ATB[i].Key - time, ATB[i].Value);
                }
                foreach (var ATBunit in allUnits)
                {
                    ATBunit.UnitATB += ATBunit.UnitInitiative * time;
                    if (ATBunit.UnitATB >= 100)
                    {
                        ATBunit.UnitATB -= 100;
                        ATB.Add(KeyValuePair.Create(0d, ATBunit));
                    }
                    ATBunit.UnitTime = (100 - ATBunit.UnitATB) / ATBunit.UnitInitiative;
                }
                for (int i = 0; i < ATB.Count; i++)
                {
                    if (ATB[i].Value == unit)
                    {
                        ATB[i] = new KeyValuePair<double, BaseUnit>(ATB[i].Key + 50 / unit.UnitInitiative, ATB[i].Value);
                    }
                }
                var x = ATB.FirstOrDefault();
                ATB.Sort((x, y) => -y.Key.CompareTo(x.Key));
                allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));

                MenuManager.Instance.ShowUnitsPortraits();
            }
        }

        public void StartTurn(BaseUnit unit)
        {
            MenuManager.Instance.SetTimer(unit);

            unit.UnitResponse = true;
            unit.UnitAdditionalDefence = 0;
            unit.GetComponent<SpriteRenderer>().sortingOrder = 2;

            UnitManager.Instance.CalculateProbabilityStats(unit);

            if (unit.Side == Side.Player)
            {
                UnitManager.Instance.SetSelectedHero(unit);
                Tile.Instance.SetHighlight(UnitManager.Instance.SelectedHero);
                GameManager.Instance.ChangeState(GameState.PlayerTurn);
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.EnemyTurn);
            }
        }

        public void EndTurn(BaseUnit unit)
        {
            Tile.Instance.DeleteHighlight();

            unit.GetComponent<SpriteRenderer>().sortingOrder = 1;

            if (GameManager.Instance.GameState == GameState.PlayerTurn)
            {
                UnitManager.Instance.SetSelectedHero(null);
            }
            bool morale = false;
            if (unit.UnitAdditionalDefence == 0 && unit.UnitCount > 0)
            {
                morale = UnitManager.Instance.Morale(unit);
            }
            if (!morale)
            {
                UpdateATB(unit);
                if (SpawnManager.Instance.EnemyUnits.Count == 0)
                {
                    GameManager.Instance.ChangeState(GameState.GameOver);
                    MenuManager.Instance.WinPanel();
                }
                else if (SpawnManager.Instance.PlayerUnits.Count == 0)
                {
                    GameManager.Instance.ChangeState(GameState.GameOver);
                    MenuManager.Instance.LosePanel();
                }
                else
                {
                    StartTurn(ATB.FirstOrDefault().Value);
                }
            }
        }

        public async UniTaskVoid TimeCounter(TextMeshProUGUI text, BaseUnit unit, CancellationToken token)
        {
            int startTime = 30;
            while (!token.IsCancellationRequested)
            {
                startTime -= 1;
                text.text = startTime.ToString();

                if (startTime <= 0)
                {
                    EndTurn(unit);
                    break;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
