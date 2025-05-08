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
using System;

public sealed class GridsPresenter : MonoBehaviour
{
    private const string GridContainerName = "GridContainer";
    private const float DefaultZPosition = 1f;
    private const float DefaultScaleZ = 1f;
    private const int GridIndexOffset = 1;
    private const float HalfCellOffset = 0.5f;
    private const float DefaultPanelInsetPx = 0f;

    [Header("Tile Prefabs")]
    [SerializeField] private TileView _grassPrefab;
    [SerializeField] private TileView _mountainPrefab;
    [SerializeField] private TileView _lakePrefab;

    [Header("UI‑элементы (нужны для адаптивного размера сетки)")]
    [SerializeField] private RectTransform _leftPanel;
    [SerializeField] private RectTransform _rightPanel;

    public UniTask WhenReady => _ready.Task;
    private readonly UniTaskCompletionSource _ready = new();

    public Vector3 GridScale => _gridContainer.localScale;
    public readonly Subject<Vector3> OnGridScaleChanged = new();
    private Transform _gridContainer;

    private readonly Dictionary<(int, int), TileView> _tiles = new();

    [Inject] private readonly DiContainer _container;
    [Inject] private readonly IGridsService _gridService;

    private readonly CompositeDisposable _disposables = new();

    private void OnEnable()
    {
        BindStreams();
    }

    private void OnDestroy() => _disposables.Dispose();

    public TileView GetTile(int x, int y) => _tiles.TryGetValue((x, y), out var tile) ? tile : null;
    public TileView GetTile(Guid unitId) => _tiles.Values.FirstOrDefault(tile => tile.Data.OccupiedUnitId == unitId);
    public int GetDistance(Tile a, Tile b) => _gridService.GetDistance(a, b);

    public List<TileView> GetNeighbours(int x, int y, bool includeDiagonals = true)
    {
        var offsets = includeDiagonals
            ? new (int dx, int dy)[]
            {
                ( 1,  0), ( 1, -1), ( 0, -1), (-1, -1),
                (-1,  0), (-1,  1), ( 0,  1), ( 1,  1)
            }
            : new (int dx, int dy)[]
            {
                ( 1, 0), ( 0, -1), (-1, 0), ( 0, 1)
            };

        var neighbours = new List<TileView>();

        foreach (var (dx, dy) in offsets)
        {
            if (_tiles.TryGetValue((x + dx, y + dy), out var neighbour))
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private void BindStreams()
    {
        _gridService.OnGridGenerated
            .Subscribe(tiles => GenerateGrid(tiles).Forget())
            .AddTo(_disposables);
    }

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
                TerrainType.Mountain => _mountainPrefab,
                TerrainType.Lake => _lakePrefab,
                _ => _grassPrefab
            };

            var view = _container.InstantiatePrefabForComponent<TileView>(prefab, _gridContainer, new object[] { tile });
            view.transform.localPosition = new Vector3(tile.X, tile.Y, DefaultZPosition);
            _tiles[(tile.X, tile.Y)] = view;
        }

        var gridSize = new Vector2Int(tiles.Max(t => t.X) + GridIndexOffset, tiles.Max(t => t.Y) + GridIndexOffset);

        FitAndPositionGrid(gridSize);

        _ready.TrySetResult();
    }

    private void FitAndPositionGrid(Vector2Int gridSize)
    {
        var canvas = _leftPanel.GetComponentInParent<Canvas>();
        float leftPx = _leftPanel.rect.width * canvas.scaleFactor;
        float rightPx = _rightPanel.rect.width * canvas.scaleFactor;

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
        OnGridScaleChanged.OnNext(_gridContainer.localScale);
    }
}
