using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using UnscriptedLogic;
using UnscriptedLogic.Experimental.Generation;
using UnscriptedEngine;
using System.Collections;

public class HeatMapNode
{
    public Vector3 position;
    public float shortestDistance;
    public Vector3 closestPathPosition;

    public HeatMapNode(Vector3 position)
    {
        this.position = position;
        shortestDistance = Mathf.Infinity;
    }
}

public class GM_TowerDefenceGameMode : UGameModeBase
{
    [Header("Seed")]
    [SerializeField] private bool generateRandom;
    [SerializeField] private bool logCurrentSeed;
    [SerializeField] private int seed;

    [Header("Grid Settings")]
    [SerializeField] private GameObject[] nodePrefabs;
    [SerializeField] private GridSettings grid;

    private GridLogic<GameObject> gridGenerator;
    private List<Cell> gridWithoutPath;

    [Header("Path Settings")]
    [SerializeField] private bool drawPoints;
    [SerializeField] private GameObject[] pathwayNodes;
    [SerializeField] private float pointDist = 4;

    private List<Vector2> distributedPoints;
    private List<Vector3> assortedPoints;
    private Roy_T.AStar.Grids.Grid pathfinderGrid;

    private List<Cell> pathwayCells;
    public List<Vector3> Pathway { get; private set; }

    [Header("Debri Settings")]
    [SerializeField] private int amount;
    [SerializeField] private float verticalOffset;
    [SerializeField] private Transform parent;
    [SerializeField] private DebriThemeSO debriThemeSO;
    [SerializeField] private LayerMask debriLayer;

    [Header("Heat Map")]
    [Tooltip("Shows the heat map of the node to path")]
    [SerializeField] private bool generateHeatMap;
    [SerializeField] private Gradient heatmapColour;
    [SerializeField] private Transform heatmapParent;

    private List<HeatMapNode> heatmapNodes;

    protected override IEnumerator Start()
    {
        yield return base.Start();

        if (generateRandom)
            seed = System.DateTime.Now.Millisecond;

        UnityEngine.Random.InitState(seed);

        if (logCurrentSeed)
            Debug.Log(UnityEngine.Random.state);

        GenerateMapGrid();
        GenerateMapPath();
        GenerateHeatMap();
        GenerateMapDebri();
    }

    private void GenerateMapGrid()
    {
        gridWithoutPath = new List<Cell>();
        gridGenerator = new GridLogic<GameObject>(grid);
        gridGenerator.CreateGrid((cell, pos) =>
        {
            GameObject item = Instantiate(RandomLogic.FromArray(nodePrefabs));
            item.name = $"{cell.GridCoords.x},{cell.GridCoords.y}";
            item.transform.position = new Vector3(pos.x, 0f, pos.y);
            item.transform.forward = RandomLogic.VectorDirAroundY();
            item.transform.localScale = Vector3.one * grid.CellScale;
            item.transform.SetParent(transform);

            gridGenerator.gridCells.Add(cell, item);
            gridWithoutPath.Add(cell);
        });
    }

    private void GenerateMapPath()
    {
        //Point distribution
        distributedPoints = PoissonDiscSampling.GeneratePoints(pointDist, grid.Size);

        if (distributedPoints.Count == 0)
        {
            GenerateMapPath();
            return;
        }

        //Point Arrangment
        List<Vector3> pointPositions = new List<Vector3>();
        for (int i = 0; i < distributedPoints.Count; i++)
        {
            pointPositions.Add(new Vector3(distributedPoints[i].x, 0f, distributedPoints[i].y));
        }
        assortedPoints = PointsConnectorShort.ConnectPoints(pointPositions);

        //Pathfinder grid setup
        GridSize gridSize = new GridSize(columns: grid.Size.x, rows: grid.Size.y);
        Size cellSize = new Size(Distance.FromMeters(1), Distance.FromMeters(1));
        Velocity traversalVelocity = Velocity.FromKilometersPerHour(100);
        pathfinderGrid = Roy_T.AStar.Grids.Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);
        PathFinder pathFinder = new PathFinder();

        pathwayCells = new List<Cell>();
        for (int i = 0; i < assortedPoints.Count; i++)
        {
            if (i - 1 >= 0)
            {
                GridPosition start = new GridPosition((int)assortedPoints[i - 1].x, (int)assortedPoints[i - 1].z);
                GridPosition end = new GridPosition((int)assortedPoints[i].x, (int)assortedPoints[i].z);
                Path path = pathFinder.FindPath(start, end, pathfinderGrid);

                //gridGenerator.GetCellFromGrid((int)path.Edges[0].Start.Position.X, (int)path.Edges[0].Start.Position.Y);
                for (int j = 0; j < path.Edges.Count; j++)
                {
                    Cell cell = gridGenerator.GetCellFromGrid((int)path.Edges[j].End.Position.X, (int)path.Edges[j].End.Position.Y);
                    pathwayCells.Add(cell);
                }
            }
        }

        CleanUpPath(pathwayCells);
    }

    private void CleanUpPath(List<Cell> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            //Cleans up back tracks
            if (i >= 2)
            {
                if (path[i].WorldCoords == path[i - 2].WorldCoords)
                {
                    path.RemoveAt(i - 1);
                }
            }
        }

        int safetyCatch = 0;
        bool hasRoundabouts = true;
        while (hasRoundabouts || safetyCatch < 25)
        {
            hasRoundabouts = false;
            for (int i = 0; i < path.Count; i++)
            {
                if (i >= 4)
                {
                    //Potential 4 point roundabout
                    if (Vector3.Distance(path[i].WorldCoords, path[i - 3].WorldCoords) <= gridGenerator.Settings.CellSpacing + 0.1f)
                    {
                        //Debug.Log(Vector3.Distance(path[i].WorldCoords, path[i - 3].WorldCoords) <= gridGenerator.Settings.CellSpacing + 0.1f, gridGenerator.gridCells[path[i]]);

                        path.RemoveAt(i - 1);
                        path.RemoveAt(i - 2);

                        hasRoundabouts = true;
                        break;
                    }
                }
            }

            safetyCatch++;
        }

        //Create path visual after clean up
        Pathway = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            if (pathwayNodes.Length > 0)
            {
                if (i == 0 || i == path.Count - 1) continue;

                Vector3 dir = (path[i].WorldCoords - path[i + 1].WorldCoords).normalized;
                GameObject pathNode = Instantiate(pathwayNodes[0], new Vector3(path[i].WorldCoords.x, 0f, path[i].WorldCoords.y), Quaternion.identity, transform);
                pathNode.transform.forward = new Vector3(dir.x, 0f, dir.y);
            }

            gridWithoutPath.Remove(path[i]);
            gridGenerator.gridCells[path[i]].SetActive(false);
            Pathway.Add(new Vector3(path[i].WorldCoords.x, 0f, path[i].WorldCoords.y));
        }
    }

    private void GenerateMapDebri()
    {
        DebriGenerator.GenerateDebrisRandomly(new DebriGenSettings()
        {
            amount = amount,
            verticalOffset = verticalOffset,
            parent = parent,
            pathlist = Pathway,
            nodes = GetAllNonePathNodePositions(),
            debriTheme = debriThemeSO,
            debriLayer = debriLayer,
            heatmap = heatmapNodes
        });
    }

    private List<Vector3> GetAllNonePathNodePositions()
    {
        List<Vector3> nodes = new List<Vector3>();
        for (int i = 0; i < gridGenerator.gridCells.Count; i++)
        {
            GameObject nodeObject = gridGenerator.gridCells.ElementAt(i).Value;
            if (nodeObject.activeInHierarchy)
            {
                nodes.Add(nodeObject.transform.position);
            }
        }

        return nodes;
    }

    private void GenerateHeatMap()
    {
        float lowestValue = Mathf.Infinity;
        float highestValue = 0f;

        heatmapNodes = new List<HeatMapNode>();

        for (int i = 0; i < gridWithoutPath.Count; i++)
        {
            Vector3 nodePos = new Vector3(gridWithoutPath[i].WorldCoords.x, 0f, gridWithoutPath[i].WorldCoords.y);
            HeatMapNode heatMapNode = new HeatMapNode(nodePos);

            for (int j = 0; j < Pathway.Count; j++)
            {
                float distance = Vector3.Distance(nodePos, Pathway[j]);
                if (distance < heatMapNode.shortestDistance)
                {
                    heatMapNode.shortestDistance = distance;
                    heatMapNode.closestPathPosition = Pathway[j];
                }
            }

            if (heatMapNode.shortestDistance < lowestValue)
            {
                lowestValue = heatMapNode.shortestDistance;
            }

            if (heatMapNode.shortestDistance > highestValue)
            {
                highestValue = heatMapNode.shortestDistance;
            }

            heatmapNodes.Add(heatMapNode);
        }

        if (generateHeatMap)
        {
            highestValue -= lowestValue;
            for (int i = 0; i < heatmapNodes.Count; i++)
            {
                float percentage = (heatmapNodes[i].shortestDistance - lowestValue) / highestValue;
                GameObject heatmapNodeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                heatmapNodeObject.transform.position = heatmapNodes[i].position;
                heatmapNodeObject.transform.SetParent(heatmapParent);
                heatmapNodeObject.GetComponent<Renderer>().material.color = heatmapColour.Evaluate(percentage);
                Destroy(heatmapNodeObject.GetComponent<BoxCollider>());
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawPoints) return;
        if (assortedPoints == null)
            return;

        for (int i = 0; i < assortedPoints.Count; i++)
        {
            Gizmos.DrawSphere(assortedPoints[i] - new Vector3(grid.Size.x / 2f, -2f, grid.Size.x / 2f), 0.25f);

            if (i - 1 >= 0)
            {
                Gizmos.DrawLine(assortedPoints[i - 1] - new Vector3(grid.Size.x / 2f, -2f, grid.Size.x / 2f), assortedPoints[i] - new Vector3(grid.Size.x / 2f, -2f, grid.Size.x / 2f));
            }
        }
    }
}