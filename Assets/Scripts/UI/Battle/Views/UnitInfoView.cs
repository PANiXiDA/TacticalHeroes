using System.Linq;

using Assets.Scripts.GameEngine.Domain;

using TMPro;

using UnityEngine;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class UnitInfoView : MonoBehaviour
    {
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

        public void Show(Unit unit)
        {
            _name.text = unit.Name;
            _attack.text = unit.Attack.ToString();
            _defence.text = unit.Defence.ToString();
            _health.text = $"{unit.CurrentHealth}/{unit.FullHealth}";
            _arrows.text = unit.Arrows.ToString();
            _range.text = unit.Range.ToString();
            _damage.text = $"{unit.MinDamage}-{unit.MaxDamage}";
            _speed.text = unit.Speed.ToString();
            _initiative.text = unit.Initiative.ToString();
            _morale.text = unit.Morale.ToString();
            _luck.text = unit.Luck.ToString();
            _abilities.text = string.Join(", ",unit.Abilities.Select(ability => ability.Type.GetDisplayName()));

            _infoPanel.SetActive(true);
        }

        public void Hide() => _infoPanel.SetActive(false);
    }
}
