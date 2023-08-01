using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] List<Sprite> Candies = new List<Sprite>();
    GameObject[,] Grid;
    [SerializeField] GameObject TilePrefab;
    [SerializeField] Vector2Int range;
    [SerializeField] float distance = 1.0f;
    [SerializeField] float timeToSwap = .5f;
    [SerializeField] float timeToMove = .25f;

    public static int CountSlides;
    public static GridManager Instance;

    public bool IsSwaping { private set; get; }
    
    void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }

    private void Start()
    {
        CreateGrid();
        CountSlides = 0;
    }

    //Generate Grid of Candies
    void CreateGrid()
    {
        transform.position = new Vector2(((range.x * distance) / 2 - distance / 2) * transform.localScale.x, (range.y * distance / 2 - distance / 2) * transform.localScale.y) * -1;

        Grid = new GameObject[range.x, range.y];

        for (int i = 0; i < range.x; i++)
        {
            for (int j = 0; j < range.y; j++)
            {
                GameObject candy = SpawnCandy(new Vector2Int(i, j));

                candy.transform.localPosition = new Vector2(i * distance, j * distance);
                Grid[i, j] = candy;
            }
        }
    }

    GameObject SpawnCandy(Vector2Int coord)
    {
        List<Sprite> possibleSprites = HorizontalAlignmentCheck(Candies, coord);
        possibleSprites = VerticalAlignmentCheck(possibleSprites, coord);

        Sprite sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];
        GameObject candy = Instantiate(TilePrefab, transform);
        candy.name = sprite.name;
        candy.GetComponent<SpriteRenderer>().sprite = sprite;
        candy.GetComponent<Tile>().position = coord;

        return candy;
    }

    List<Sprite> HorizontalAlignmentCheck(List<Sprite> sprites, Vector2Int coord)
    {
        List<Sprite> newSprites = new List<Sprite>(sprites);

        Sprite candy1 = GetSpriteAt(new Vector2Int(coord.x - 1, coord.y));
        Sprite candy2 = GetSpriteAt(new Vector2Int(coord.x - 2, coord.y));

        if (candy1 != null && candy1 == candy2)
        {
            newSprites.Remove(candy1);
        }

        return newSprites;
    }

    List<Sprite> VerticalAlignmentCheck(List<Sprite> sprites, Vector2Int coord)
    {
        List<Sprite> newSprites = new List<Sprite>(sprites);

        Sprite candy1 = GetSpriteAt(new Vector2Int(coord.x, coord.y - 1));
        Sprite candy2 = GetSpriteAt(new Vector2Int(coord.x, coord.y - 2));

        if (candy1 != null && candy1 == candy2)
        {
            newSprites.Remove(candy1);
        }

        return newSprites;
    }

    Sprite GetSpriteAt(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= range.x
            ||
            coord.y < 0 || coord.y >= range.y)
            return null;

        GameObject tile = Grid[coord.x, coord.y];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

        return renderer.sprite;
    }

    //Swap two candies
    public void Swap(Vector2Int pos1, Vector2Int pos2)
    {
        IsSwaping = true;
        StartCoroutine(SwapCoroutine(pos1, pos2));
    }

    IEnumerator SwapCoroutine(Vector2Int pos1, Vector2Int pos2)
    {
        yield return StartCoroutine(SwapTiles(pos1, pos2));

        bool check = CheckMatches();
        if (!check)
        {
            yield return StartCoroutine(SwapTiles(pos1, pos2));

            IsSwaping = false;
            yield break;
        }

        do
        {
            List<EmptyCell> cells = GetHolesToFill();

            yield return StartCoroutine(MoveAll(cells));

            check = CheckMatches();
            yield return null;

        } while (check);
        
        IsSwaping = false;
        yield break;
    }

    IEnumerator SwapTiles(Vector2Int pos1, Vector2Int pos2)
    {
        GameObject tile1 = Grid[pos1.x, pos1.y];
        Tile t1 = tile1.GetComponent<Tile>();

        GameObject tile2 = Grid[pos2.x, pos2.y];
        Tile t2 = tile2.GetComponent<Tile>();

        float timer = 0;
        while (timer <= timeToSwap)
        {
            tile1.transform.localPosition = Vector2.Lerp(pos1, pos2, timer / timeToSwap);
            tile2.transform.localPosition = Vector2.Lerp(pos2, pos1, timer / timeToSwap);

            timer += Time.deltaTime;
            yield return null;
        }

        tile1.transform.localPosition = new Vector2(pos2.x, pos2.y);
        tile2.transform.localPosition = new Vector2(pos1.x, pos1.y);

        Vector2Int temp = pos1;

        Grid[pos1.x, pos1.y] = tile2;
        t1.position = pos2;

        Grid[pos2.x, pos2.y] = tile1;
        t2.position = temp;

        yield break;
    }

    //Check Match3 of Grid candies
    bool CheckMatches()
    {
        Debug.Log("CheckMatches start");
        HashSet<GameObject> matchedTiles = new HashSet<GameObject>();

        for (int i = 0; i < range.x; i++)
        {
            for (int j = 0; j < range.y; j++)
            {
                GameObject tile = Grid[i, j];

                List<GameObject> horizontalMatches = FindHorizontalMatchForTile(new Vector2Int(i, j), tile);
                List<GameObject> verticalMatches = FindVerticalMatchForTile(new Vector2Int(i, j), tile);

                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(tile);
                    GameManager.Score += 10;
                }
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(tile);
                    GameManager.Score += 10;
                }
            }
        }

        foreach (GameObject tile in matchedTiles)
        {
            Tile t = tile.GetComponent<Tile>();
            Grid[t.position.x, t.position.y] = null;

            Destroy(tile);
        }

        Debug.Log("CheckMatches end");
        return matchedTiles.Count >= 2;
    }

    List<GameObject> FindHorizontalMatchForTile(Vector2Int coord, GameObject tile)
    {
        List<GameObject> matches = new List<GameObject>();

        for (int j = coord.y + 1; j < range.y; j++)
        {
            GameObject match = Grid[coord.x, j];
            if (tile.name != match.name)
            {
                break;
            }

            matches.Add(match);
        }

        return matches;
    }

    List<GameObject> FindVerticalMatchForTile(Vector2Int coord, GameObject tile)
    {
        List<GameObject> matches = new List<GameObject>();

        for (int i = coord.x + 1; i < range.y; i++)
        {
            GameObject match = Grid[i, coord.y];
            if (tile.name != match.name)
            {
                break;
            }

            matches.Add(match);
        }

        return matches;
    }

    // Fill empty cells in Grid
    List<EmptyCell> GetHolesToFill()
    {
        List<EmptyCell>  cells = new List<EmptyCell>();

        for (int i = 0; i < range.x; i++)
        {
            int offset = 0;

            for (int j = 0; j < range.y; j++)
            {
                if (Grid[i, j] == null)
                {
                    bool isFound = false;
                    for (int filler = j; filler < range.y - 1; filler++)
                    {
                        GameObject nextTile = Grid[i, filler + 1];

                        if (nextTile == null) continue;

                        isFound = true;

                        Grid[i, j] = nextTile;
                        Grid[i, filler + 1] = null;

                        Tile t = nextTile.GetComponent<Tile>();
                        t.position = new Vector2Int(i, j);

                        cells.Add(new EmptyCell(new float[] { i * distance, (filler + 1) * distance, i * distance, j * distance }, nextTile));
                        break;
                    }

                    if (!isFound)
                    {
                        GameObject newTile = SpawnRandomCandy(new Vector2Int(i, j));
                        Grid[i, j] = newTile;

                        Tile t = newTile.GetComponent<Tile>();
                        t.position = new Vector2Int(i, j);

                        cells.Add(new EmptyCell(new float[] { i * distance, (range.y + offset) * distance, i * distance, j * distance }, newTile));

                        offset++;
                    }
                }
            }
        }

        return cells;
    }

    GameObject SpawnRandomCandy(Vector2Int coord)
    {
        int random = Random.Range(0, Candies.Count);

        Sprite sprite = Candies[random];
        GameObject candy = Instantiate(TilePrefab, transform);
        candy.name = sprite.name;
        candy.GetComponent<SpriteRenderer>().sprite = sprite;
        candy.GetComponent<Tile>().position = coord;

        return candy;
    }

    IEnumerator MoveAll(List<EmptyCell> cells)
    {
        float timer = 0;
        while (timer <= timeToMove)
        {
            foreach (EmptyCell cell in cells)
            {
                float y = Mathf.Lerp(cell.pos1.y, cell.pos2.y, timer / timeToMove);
                cell.tile.transform.localPosition = new Vector2(cell.pos2.x, y);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        foreach (EmptyCell cell in cells)
        {
            cell.tile.transform.localPosition = new Vector2(cell.pos2.x, cell.pos2.y);
        }

        yield break;
    }

    class EmptyCell
    {
        public Vector2 pos1;
        public Vector2 pos2;
        public GameObject tile;

        public EmptyCell(Vector2 pos1, Vector2 pos2, GameObject tile)
        {
            this.pos1 = pos1;
            this.pos2 = pos2;
            this.tile = tile;
        }

        public EmptyCell(float[] coordinates, GameObject tile)
        {
            pos1 = new Vector2(coordinates[0], coordinates[1]);
            pos2 = new Vector2(coordinates[2], coordinates[3]);
            this.tile = tile;
        }
    }

}
