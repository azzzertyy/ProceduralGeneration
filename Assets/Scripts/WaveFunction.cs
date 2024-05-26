using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NavMeshPlus.Components;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WaveFunction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyObject;
    [SerializeField] private int maxEnemyCount;
    [SerializeField] private NavMeshSurface navSurface;
    [SerializeField] private int maxPathTries;
    [SerializeField] private GameObject upStair;
    [SerializeField] private GameObject downStair;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Tile borderTile;
    [SerializeField] private Cell cellObj;
    public event Action GenerationCompleted;

    [Header("WFC Variables")]
    public int width;
    public int height;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public List<Cell> validCells;
    private bool completeFirst = false;
    private bool[,] visitedPositions;
    private int currentEnemyCount = 0;
    private GameObject startTile;
    private bool gridInitializing = false;

    void Awake()
    {
        visitedPositions = new bool[width, height];
        gridComponents = new List<Cell>();
        InitializeGrid();
        GenerationCompleted = OnCompletion;
    }
    #region PostProcessing
    void PostProcessing()
    {
        BakeNavmesh();
        GenerateGoal();
        SpawnAllEnemies();
        SpawnPlayer();
    }
    void SpawnPlayer()
    {
        if(startTile != null)
        {
            Instantiate(playerObject, startTile.transform.position, Quaternion.identity);
        }
    }
    void BakeNavmesh()
    {
        navSurface.BuildNavMesh();
    }
    #region Goal
    void GenerateGoal()
    {
        validCells = gridComponents.Where(c => !c.tileOptions.Contains(borderTile) && c.collapsed).ToList();
        if (validCells.Count < 2)
        {
            Debug.LogError("Not enough valid cells to place start and end goals.");
            return;
        }

        bool possiblePath = false;
        int tries = 0;
        NavMeshPath path = new NavMeshPath();

        while(possiblePath == false && tries < maxPathTries)
        {
            Debug.Log(tries);
            tries++;

            Cell startCell = validCells[UnityEngine.Random.Range(0, validCells.Count)];
            Cell endCell = validCells[UnityEngine.Random.Range(0, validCells.Count)];
            while (endCell == startCell)
            {
                endCell = validCells[UnityEngine.Random.Range(0, validCells.Count)];
            }
            if (NavMesh.CalculatePath(startCell.transform.position, endCell.transform.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                possiblePath = true;
                startTile = Instantiate(upStair, startCell.transform.position, Quaternion.identity);
                startCell.isLadder = true;
                Instantiate(downStair, endCell.transform.position, Quaternion.identity);
                endCell.isLadder = true;
            }
        }
        if(tries >= maxPathTries)
        {
            Debug.LogError("Couldn't find a possible path!");
            Restart();
        }
        else
        {
            Debug.Log("Found path between entrance and exit!");
        }
    }

    #endregion
    #region EnemyGeneration
    void SpawnAllEnemies()
    {
        maxEnemyCount = gridComponents.Count() / 10;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                
                SpawnEnemy(x, y);
            }
        }
        
    }

    void SpawnEnemy(int x, int y)
    {
        if (currentEnemyCount < maxEnemyCount)
        {
            Cell targetCell = gridComponents[x + y * width];
            if (targetCell.tileOptions.Contains(borderTile) || targetCell.isLadder)
            {
                return;
            }
            float randomChance = UnityEngine.Random.value;
            float spawnProbability = 0.1f;
            if (randomChance <= spawnProbability)
            {
                currentEnemyCount += 1;
                Enemy newEnemy = Instantiate(enemyObject, targetCell.transform.position, Quaternion.identity).GetComponent<Enemy>();
            }
        }
    }
    #endregion
    #endregion
    #region WFCAlgorithm

    void OnCompletion()
    {
        if (!completeFirst)
        {
            completeFirst = true;
            Debug.Log("Complete!");
            PostProcessing();
        }
    }

    void InitializeGrid()
    {
        if(gridInitializing)
        {
            return;
        }
        gridInitializing = true;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity, transform);
                if (IsBorder(x, y))
                {
                    newCell.CreateCell(true, new Tile[] { borderTile });
                    Instantiate(borderTile, new Vector2(x, y), Quaternion.identity, newCell.transform);
                }
                else
                {
                    newCell.CreateCell(false, tileObjects);
                }
                gridComponents.Add(newCell);
            }
        }

        Propagate(gridComponents[0]);
        StartCoroutine(CalculateEntropy());
    }

    bool IsBorder(int x, int y)
    {
        return x == 0 || y == 0 || x == width - 1 || y == height - 1;
    }

    IEnumerator CalculateEntropy()
    {
        List<Cell> tempGrid = GetUncollapsedCells();

        if (tempGrid.Count == 0)
        {
            OnGenerationCompleted();
            yield break;
        }

        Cell minOptionsCell = tempGrid.OrderBy(c => c.tileOptions.Length).First();
        int minOptions = minOptionsCell.tileOptions.Length;

        tempGrid.RemoveAll(c => c.tileOptions.Length > minOptions);

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<Cell> tempGrid)
    {
        if (tempGrid == null || tempGrid.Count == 0)
        {
            Debug.LogError("No cells available for collapse.");
            return;
        }

        Cell cellToCollapse = tempGrid.First();

        if (cellToCollapse.tileOptions.Count() == 0)
        {
            Debug.LogError("No tile options available for the selected cell.");
            Restart();
            return;
        }

        float totalWeight = cellToCollapse.tileOptions.Sum(tile => tile.weight);
        float randomWeight = UnityEngine.Random.Range(0f, totalWeight);
        float weightSum = 0f;
        Tile selectedTile = null;

        foreach (Tile tileOption in cellToCollapse.tileOptions)
        {
            weightSum += tileOption.weight;
            if (randomWeight <= weightSum)
            {
                selectedTile = tileOption;
                break;
            }
        }

        if (selectedTile == null)
        {
            selectedTile = cellToCollapse.tileOptions[0];
        }

        cellToCollapse.collapsed = true;
        cellToCollapse.tileOptions = new List<Tile> { selectedTile }.ToArray();

        Instantiate(selectedTile, cellToCollapse.transform.position, Quaternion.identity, cellToCollapse.transform);

        Propagate(cellToCollapse);
    }




    void Propagate(Cell startCell)
    {
        Queue<Cell> cellsToVisit = new Queue<Cell>();
        cellsToVisit.Enqueue(startCell);

        ClearVisitedPositions();

        while (cellsToVisit.Count > 0)
        {
            Cell currentCell = cellsToVisit.Dequeue();
            Vector2Int currentPosition = GetCurrentPosition(currentCell);
            List<Tile> tileOptionsList = new List<Tile>(currentCell.tileOptions);
            UpdateNeighborsOptions(currentPosition.x, currentPosition.y, tileOptionsList);
            currentCell.tileOptions = tileOptionsList.ToArray();
            visitedPositions[currentPosition.x, currentPosition.y] = true;

            AddUnvisitedNeighbors(currentPosition.x, currentPosition.y, cellsToVisit);
        }

        StartCoroutine(CalculateEntropy());
    }

    void ClearVisitedPositions()
    {
        Array.Clear(visitedPositions, 0, visitedPositions.Length);
    }

    void AddUnvisitedNeighbors(int x, int y, Queue<Cell> cellsToVisit)
    {
        AddNeighborToVisit(x, y - 1, cellsToVisit);
        AddNeighborToVisit(x + 1, y, cellsToVisit);
        AddNeighborToVisit(x, y + 1, cellsToVisit);
        AddNeighborToVisit(x - 1, y, cellsToVisit);
    }

    void AddNeighborToVisit(int x, int y, Queue<Cell> cellsToVisit)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && !visitedPositions[x, y])
        {
            Cell neighborCell = gridComponents[x + y * width];
            cellsToVisit.Enqueue(neighborCell);
            visitedPositions[x, y] = true;
        }
    }

    void UpdateNeighborsOptions(int x, int y, List<Tile> options)
    {
        UpdateNeighborOptions(x, y - 1, options, tile => tile.upNeighbours);
        UpdateNeighborOptions(x + 1, y, options, tile => tile.leftNeighbours);
        UpdateNeighborOptions(x, y + 1, options, tile => tile.downNeighbours);
        UpdateNeighborOptions(x - 1, y, options, tile => tile.rightNeighbours);
    }

    void UpdateNeighborOptions(int x, int y, List<Tile> options, Func<Tile, Tile[]> neighborGetter)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            Cell neighborCell = gridComponents[x + y * width];
            List<Tile> validOptions = new List<Tile>();

            foreach (Tile possibleOption in neighborCell.tileOptions)
            {
                validOptions.AddRange(neighborGetter(possibleOption));
            }

            CheckValidity(options, validOptions);
        }
    }

    Vector2Int GetCurrentPosition(Cell cell)
    {
        Vector3 cellPosition = cell.transform.position;
        int x = Mathf.RoundToInt(cellPosition.x);
        int y = Mathf.RoundToInt(cellPosition.y);
        return new Vector2Int(x, y);
    }

    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        optionList.RemoveAll(tile => !validOption.Contains(tile));
    }

    List<Cell> GetUncollapsedCells()
    {
        return gridComponents.FindAll(c => !c.collapsed);
    }

    void OnGenerationCompleted()
    {
        GenerationCompleted?.Invoke();
    }
    #endregion
    void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
