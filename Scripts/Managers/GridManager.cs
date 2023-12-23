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
                //var randomTile = Random.Range(0, 30) == 3 ? _mountainTile : _grassTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                // Instantiate - озвол€ет разработчикам игр динамически создавать и размещать объекты в своих играх. 
                // Instantiate(prefab, position, rotation); 
                // prefab - название используемого префаба (шаблона)
                // position - координата размещени€
                // rotation -  вращени€. с помощью Quaternion.identity задаетс€ нулевое вращение.

                spawnedTile.name = $"Tile {x} {y}"; //название и координаты каждой плитки пол€

                _tiles.Add(new Vector2(x, y), spawnedTile);
            }
        }
        LinesForGrid();


        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }
    public void LinesForGrid()
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 66;
        int i = 0;
        for (int x = 0; x < _width; x++)
        {
            line.SetPosition(i, new Vector3(x - 0.5f, -0.5f, -1));
            line.SetPosition(i++, new Vector3(x - 0.5f, -0.5f + _height, -1));
            line.SetPosition(i++, new Vector3(x - 0.5f + 1, -0.5f + _height, -1));
            line.SetPosition(i++, new Vector3(x - 0.5f + 1, -0.5f, -1));
        }
        for (int y = 0; y < _height; y++)
        {
            line.SetPosition(i, new Vector3(-0.5f + _width, y - 0.5f, -1));
            line.SetPosition(i++, new Vector3(-0.5f, y - 0.5f, -1));
            line.SetPosition(i++, new Vector3(-0.5f, y - 0.5f + 1, -1));
            line.SetPosition(i++, new Vector3(-0.5f + _width, y - 0.5f + 1, -1));
        }
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;
        line.startColor = Color.black;
        line.endColor = Color.black;

        //Ќе видно среднюю линию, дорисовываем
        //line.SetPosition(line.positionCount-1, new Vector3(0.5f + _width/2, _height - 0.5f, -1));
        //line.SetPosition(line.positionCount, new Vector3(0.5f + _width / 2, 0, -1));
        //line.material = new Material(Shader.Find("Sprites/Default"));
        //line.startWidth = 0.02f;
        //line.endWidth = 0.02f;
        //line.startColor = Color.black;
        //line.endColor = Color.black;


        //for (int x = 0; x <= _width; x++)
        //{
        //    if (x % 2 == 0)
        //    { // draw vertical line from bottom to top
        //        line.SetPosition(i, new Vector3(x - 0.5f, -0.5f, -1));
        //        line.SetPosition(i++, new Vector3(x - 0.5f, -0.5f + _height, -1));
        //        line.SetPosition(i++, new Vector3(x - 0.5f + 1, -0.5f + _height, -1));
        //    }
        //    else
        //    { // alternate drawing vertical line from top to bottom
        //        line.SetPosition(i++, new Vector3(x - 0.5f, -0.5f + _height, -1));
        //        line.SetPosition(i++, new Vector3(x - 0.5f, -0.5f, -1));
        //        line.SetPosition(i++, new Vector3(x - 0.5f + 1, -0.5f, -1));
        //    }
        //}
        //Debug.Log(i);
        //line.startColor = Color.white;
        //line.endColor = Color.white;
        //line.startWidth = 0.1f;
        //line.endWidth = 0.1f;
        //Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        //line.material = whiteDiffuseMat;

        //LineRenderer line2 = gameObject.AddComponent<LineRenderer>();
        //line2.positionCount = 42;
        //i = 0;
        //for (int y = 0; y < _height; y++)
        //{
        //    if (y % 2 == 0)
        //    {
        //        line2.SetPosition(i, new Vector3(-0.5f + _width, y - 0.5f, -1));
        //        line2.SetPosition(i++, new Vector3(-0.5f, y - 0.5f, -1));
        //        line2.SetPosition(i++, new Vector3(-0.5f, y - 0.5f + 1, -1));
        //    }
        //    else
        //    {
        //        line2.SetPosition(i++, new Vector3(-0.5f, y - 0.5f, -1));
        //        line2.SetPosition(i, new Vector3(-0.5f + _width, y - 0.5f, -1));
        //        line2.SetPosition(i, new Vector3(-0.5f + _width, y - 0.5f+1, -1));
        //    }
        //}
        //Debug.Log(i);
        //line2.startColor = Color.white;
        //line2.endColor = Color.white;
        //line2.startWidth = 0.1f;
        //line2.endWidth = 0.1f;
        //Material whiteDiffuseMat2 = new Material(Shader.Find("Unlit/Texture"));
        //line2.material = whiteDiffuseMat2;
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
