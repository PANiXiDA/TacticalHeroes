using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int _width, _height;

    [SerializeField] public Tile _grassTile, _mountainTile, _lakeTile;

    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                int index = Random.Range(0, 50);
                var randomTile = _grassTile;
                if (index == 1)
                    randomTile = _mountainTile;
                else if(index == 2)
                    randomTile = _lakeTile;

                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);

                spawnedTile.name = $"Tile {x} {y}";

                _tiles.Add(new Vector2(x, y), spawnedTile);
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }
    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }
    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > _width - 2 && t.Value.Walkable).OrderBy(t => Random.value).First().Value;
    }
    public Vector2 GetTileCoordinate(Tile tile)
    {
        Vector2 tmp = _tiles.Where(a => a.Value == tile).FirstOrDefault().Key;
        return tmp;
    }
    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }
    public Dictionary<Vector2, Tile> GetGrid()
    {
        return _tiles;
    }
}
