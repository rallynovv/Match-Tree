using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region Singleton

    private static BoardManager _instance = null;

    public static BoardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BoardManager>();

                if (_instance == null)
                {
                    Debug.LogError("Fatal Error: BoardManager not Found");
                }
            }

            return _instance;
        }
    }

    #endregion

    [Header("Board")]
    public Vector2Int size;
    public Vector2 offsetTile;
    public Vector2 offsetBoard;

    [Header("Tile")]
    public List<Sprite> tileTypes = new List<Sprite>();
    public GameObject tilePrefab;

    
    public bool IsAnimating { get; set; }

    private Vector2 startPosition;
    private Vector2 endPosition;
    private TileController[,] tiles;

    private void Start()
    {
        Vector2 tileSize = tilePrefab.GetComponent<SpriteRenderer>().size;
        CreateBoard(tileSize);

        IsAnimating = false;
    }

    #region Board Generator

    private void CreateBoard(Vector2 tileSize)
    {
        tiles = new TileController[size.x, size.y];

        Vector2 totalSize = (tileSize + offsetTile) * (size - Vector2.one);

        startPosition = (Vector2)transform.position - (totalSize / 2) + offsetBoard;
        endPosition = startPosition + totalSize;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                TileController newTile = Instantiate(tilePrefab, new Vector2(startPosition.x + ((tileSize.x + offsetTile.x) * x), startPosition.y + ((tileSize.y + offsetTile.y) * y)), tilePrefab.transform.rotation, transform).GetComponent<TileController>();
                tiles[x, y] = newTile;
                //get no match id
                List<int> possibleId = GetStartingPossibleIdList(x, y); 
                int newId = possibleId[Random.Range(0, possibleId.Count)];

                newTile.ChangeId(newId, x, y);

            }
        }
    }
    private List<int> GetStartingPossibleIdList(int x, int y)
    {
        List<int> possibleid = new List<int>();

        for (int i = 0; i < tileTypes.Count; i++)
        {
            possibleid.Add(i);
        }
        
        if (x > 1 && tiles[x - 1, y].id == tiles[x - 2, y].id)
        {
            possibleid.Remove(tiles[x - 1, y].id);
        }

        if (y > 1 && tiles[x, y - 1].id == tiles[x, y - 2].id)
        {
            possibleid.Remove(tiles[x, y - 1].id);
        }
        return possibleid;

        Generate;

        #region Swapping 

        public IEnumerator SwapTilePosition(TileController a, TileController b, System.Action onComplete)
        {
            Vector2Int indexA = GetTileIndex(a);
            Vector2Int indexB = GetTileIndex(b);

            tiles[indexA.x, indexA.y] = b;
            tiles[indexB.x, indexA.y] = a;

            a.ChangeId(a.id, indexB.x, indexB.y);
            b.ChangeId(b.id, indexA.x, indexA.y);

            bool isRoutineACompleted = false;
            bool isRoutineBCompleted = false;

            StartCoroutine(a.MoveTilePosition(GetIndexPosition(indexB), () => { isRoutineACompleted = true; }));
            StartCoroutine(a.MoveTilePosition(GetIndexPosition(indexA), () => { isRoutineBCompleted = true; }));

            yield return new WaitUntil(() => { return isRoutineACompleted && isRoutineBCompleted; });

            onCompleted?.Invoke();
        }

        #endregion


        #region Helper

        public Vector2Int GetTileIndex(TileController tile)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (tile == tiles[x, y]) return new Vector2Int(x, y);
                }
            }

            return new Vector2Int(-1, -1);
        }

        #endregion

    }

    public Vector2 GetIndexPosition(Vector2Int index)
    {
        Vector2 tileSize = tilePrefab.GetComponent<SpriteRenderer>().size;
        return new Vector2((startPosition.x + ((tileSize.x + offsetTile.x) * index.x), startPosition.y + ((tileSize.y + offsetTile.y)) * index.y));
    }
}
    

