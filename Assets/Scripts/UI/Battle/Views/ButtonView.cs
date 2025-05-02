using Assets.Scripts.Common.Enumerations;

using UnityEngine;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class ButtonView : MonoBehaviour
    {
        [SerializeField] private BattleButtonType _type;

        public BattleButtonType GetButtonType => _type;
    }
}
