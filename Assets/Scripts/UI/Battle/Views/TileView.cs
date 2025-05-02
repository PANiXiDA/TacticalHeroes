using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using TMPro;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class TileView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private GameObject _tileForMoveHighlight, _selectedTileHighlight, _activeGameObjectHighlight;

        [Inject] public Tile Data { get; private set; }

        public void TileForMoveHighlight(bool value) => _tileForMoveHighlight.SetActive(value);
        public void SelectedTileHighlight(bool value) => _selectedTileHighlight.SetActive(value);
        public void ActiveGameObjectHighlight(bool value) => _activeGameObjectHighlight.SetActive(value);
    }
}
