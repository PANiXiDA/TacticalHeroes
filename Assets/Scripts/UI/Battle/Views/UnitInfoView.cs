using System;
using System.Linq;
using System.Text;

using Assets.Scripts.Domain.GameEngine.Domain.Core;
using Assets.Scripts.Domain.GameEngine.Domain.Enums;
using Assets.Scripts.GameEngine.Domain;

using TMPro;

using UnityEngine;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class UnitInfoView : MonoBehaviour
    {
        [SerializeField] private GameObject _container;

        [SerializeField] private GameObject _infoPanel;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _attack;
        [SerializeField] private TextMeshProUGUI _defence;
        [SerializeField] private TextMeshProUGUI _health;
        [SerializeField] private TextMeshProUGUI _arrows;
        [SerializeField] private TextMeshProUGUI _range;
        [SerializeField] private TextMeshProUGUI _damage;
        [SerializeField] private TextMeshProUGUI _speed;
        [SerializeField] private TextMeshProUGUI _initiative;
        [SerializeField] private TextMeshProUGUI _morale;
        [SerializeField] private TextMeshProUGUI _luck;
        [SerializeField] private TextMeshProUGUI _abilities;

        [SerializeField] private GameObject _effectsPanel;

        [SerializeField] private TextMeshProUGUI _effectTypes;
        [SerializeField] private TextMeshProUGUI _effectModifiers;
        [SerializeField] private TextMeshProUGUI _effectDurations;

        private void OnEnable()
        {
            _container.SetActive(true);
        }

        public void ShowInfo(Unit unit)
        {
            _effectsPanel.SetActive(false);

            _name.text = unit.Name;
            _attack.text = unit.GetStatValue(StatType.Attack).Display;
            _defence.text = unit.GetStatValue(StatType.Defence).Display;
            _health.text = $"{unit.CurrentHealth}/{unit.GetStatValue(StatType.Health).Display}";
            _arrows.text = unit.GetStatValue(StatType.Arrows).Display;
            _range.text = unit.GetStatValue(StatType.Range).Display;
            _damage.text = $"{unit.GetStatValue(StatType.MinDamage).Display}-{unit.GetStatValue(StatType.MaxDamage).Display}";
            _speed.text = unit.GetStatValue(StatType.Speed).Display;
            _initiative.text = unit.GetStatValue(StatType.Initiative).Display;
            _morale.text = unit.GetStatValue(StatType.Morale).Display;
            _luck.text = unit.GetStatValue(StatType.Luck).Display;
            _abilities.text = string.Join(", ",unit.Abilities.Select(ability => ability.Type.GetDisplayName()));

            _infoPanel.SetActive(true);
        }

        public void ShowEffects(Unit unit)
        {
            _infoPanel.SetActive(false);

            var colType = new StringBuilder();
            var colEff = new StringBuilder();
            var colTime = new StringBuilder();

            foreach (var effect in unit.Effects)
            {
                var turnsLeft = effect.Duration / 10.0;

                colType.AppendLine(effect.Type.GetDisplayName());

                var modifiers = effect.Modifiers
                    .Select(modifier => FormatModifier(modifier))
                    .ToArray();
                colEff.AppendLine(string.Join(", ", modifiers));
                colTime.AppendLine(turnsLeft.ToString("0.##"));
            }

            _effectTypes.text = colType.ToString().TrimEnd();
            _effectModifiers.text = colEff.ToString().TrimEnd();
            _effectDurations.text = colTime.ToString().TrimEnd();

            _effectsPanel.SetActive(true);
        }

        public void Hide()
        {
            _infoPanel.SetActive(false);
            _effectsPanel.SetActive(false);
        }

        private static string FormatModifier(StatModifier modifier)
        {
            string valueStr;
            if (modifier.Kind is ModifierKind.PercentAdd or ModifierKind.PercentMul)
            {
                valueStr = $"{Math.Abs(modifier.Value * 100):0}%";
            }
            else
            {
                valueStr = Math.Abs(modifier.Value).ToString("0");
            }

            bool positive = modifier.Value > 0;
            string sign = positive ? "+" : "-";
            string color = positive ? "#00FF00" : "#FF4040";

            return $"<color={color}>{sign}{valueStr} {modifier.Type.GetDisplayName()}</color>";
        }
    }
}
