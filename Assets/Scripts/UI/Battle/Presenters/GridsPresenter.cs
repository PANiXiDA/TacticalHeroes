using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using R3;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Views;
using Cysharp.Threading.Tasks;

public sealed class GridsPresenter : MonoBehaviour
{
    private const string GridContainerName = "GridContainer";
    private const float DefaultZPosition = 1f;
    private const float DefaultScaleZ = 1f;
    private const int GridIndexOffset = 1;
    private const float HalfCellOffset = 0.5f;
    private const float DefaultPanelInsetPx = 0f;

    [Header("Tile Prefabs")]
    [SerializeField] private TileView grassPrefab;
    [SerializeField] private TileView mountainPrefab;
    [SerializeField] private TileView lakePrefab;

    [Header("UI‑элементы (нужны для адаптивного размера сетки)")]
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform rightPanel;

    private Transform _gridContainer;

    private readonly Dictionary<(int, int), TileView> _tiles = new();
    private readonly CompositeDisposable _disposables = new();

    [Inject] private readonly IGridsService _gridService;

    private void OnEnable()
    {
        _gridService.OnGridGenerated
            .Subscribe(tiles => GenerateGrid(tiles).Forget())
            .AddTo(_disposables);
    }

    private void OnDestroy() => _disposables.Dispose();

    private async UniTask GenerateGrid(IReadOnlyList<Tile> tiles)
    {
        await UniTask.WaitForEndOfFrame();

        _gridContainer = new GameObject(GridContainerName).transform;
        _gridContainer.SetParent(transform, false);
        _gridContainer.localPosition = Vector3.zero;
        _gridContainer.transform.localRotation = Quaternion.identity;

        foreach (var tile in tiles)
        {
            var prefab = tile.Terrain switch
            {
                TerrainType.Mountain => mountainPrefab,
                TerrainType.Lake => lakePrefab,
                _ => grassPrefab
            };

            var inst = Instantiate(prefab, _gridContainer);
            inst.transform.localPosition = new Vector3(tile.X, tile.Y, DefaultZPosition);
            inst.Init(tile);
            _tiles[(tile.X, tile.Y)] = inst;
        }

        var gridSize = new Vector2Int(tiles.Max(t => t.X) + GridIndexOffset, tiles.Max(t => t.Y) + GridIndexOffset);

        FitAndPositionGrid(gridSize);
    }

    private void FitAndPositionGrid(Vector2Int gridSize)
    {
        var canvas = leftPanel.GetComponentInParent<Canvas>();
        float leftPx = leftPanel.rect.width * canvas.scaleFactor;
        float rightPx = rightPanel.rect.width * canvas.scaleFactor;

        float bottomPx = DefaultPanelInsetPx;
        float topPx = DefaultPanelInsetPx;

        var cam = Camera.main;
        var screenBL = new Vector3(leftPx, bottomPx, cam.nearClipPlane);
        var screenTR = new Vector3(Screen.width - rightPx, Screen.height - topPx, cam.nearClipPlane);

        var worldBL = cam.ScreenToWorldPoint(screenBL);
        var worldTR = cam.ScreenToWorldPoint(screenTR);

        float worldW = worldTR.x - worldBL.x;
        float worldH = worldTR.y - worldBL.y;

        _gridContainer.localScale = new Vector3(worldW / gridSize.x, worldH / gridSize.y, DefaultScaleZ);
        _gridContainer.position = worldBL + new Vector3((worldW / gridSize.x) * HalfCellOffset, (worldH / gridSize.y) * HalfCellOffset, DefaultZPosition);
    }

    public TileView GetView(int x, int y) => _tiles.TryGetValue((x, y), out var v) ? v : null;
}
