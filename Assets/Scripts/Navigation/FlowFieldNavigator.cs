using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FlowFieldNavigator : MonoBehaviour
{
    public static FlowFieldNavigator Instance { get; private set; }

    [Header("Grid")]
    [SerializeField] private Vector2 worldOrigin = new Vector2(-10f, -5f);
    [SerializeField] private int gridWidth = 40;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private float cellSize = 0.5f;

    [Header("Environment")]
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float obstacleCheckRadius = 0.22f;
    [Header("Runtime")]
    [SerializeField] private bool autoFollowPlayer = true;
    [SerializeField] private Transform player;

    private int cellCount;
    private bool[] walkable;
    private float[] integration; // cost-to-target
    private Vector2[] flow;      // flow vector per cell

    private Vector2 lastTargetCell = Vector2.one * float.MinValue;

    private readonly int[] neighborDX = { 0, 1, 1, 1, 0, -1, -1, -1 };
    private readonly int[] neighborDY = { 1, 1, 0, -1, -1, -1, 0, 1 };
    private readonly float[] neighborCost = { 1f, 1.41421356f, 1f, 1.41421356f, 1f, 1.41421356f, 1f, 1.41421356f };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        InitializeGrid();
    }

    private void Start()
    {
        if (player == null && autoFollowPlayer)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (autoFollowPlayer && player != null)
        {
            UpdateTarget(player.position);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void InitializeGrid()
    {
        cellCount = gridWidth * gridHeight;
        walkable = new bool[cellCount];
        integration = new float[cellCount];
        flow = new Vector2[cellCount];

        // mark obstacles
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2 world = CellCenterWorld(x, y);
                Collider2D hit = Physics2D.OverlapCircle(world, obstacleCheckRadius, environmentLayer);
                walkable[x + y * gridWidth] = hit == null;
            }
        }

        // initialize integration to infinity
        for (int i = 0; i < cellCount; i++) integration[i] = float.PositiveInfinity;

        // empty flow initially
        for (int i = 0; i < cellCount; i++) flow[i] = Vector2.zero;
    }

    // Public API: update target world position
    public void UpdateTarget(Vector2 worldTarget)
    {
        Vector2Int cell = WorldToCellInt(worldTarget);
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
        {
            // outside grid: do nothing
            return;
        }

        Vector2 targetCell = new Vector2(cell.x, cell.y);
        if (targetCell == lastTargetCell) return; // no change

        lastTargetCell = targetCell;
        ComputeIntegrationAndFlow(cell.x, cell.y);
    }

    private void ComputeIntegrationAndFlow(int targetX, int targetY)
    {
        int targetIndex = targetX + targetY * gridWidth;

        // init
        for (int i = 0; i < cellCount; i++) integration[i] = float.PositiveInfinity;

        integration[targetIndex] = 0f;

        // open list: deterministic ordering
        var open = new List<int> { targetIndex };

        while (open.Count > 0)
        {
            // find index in open with smallest integration (deterministic scan)
            int bestIdx = 0;
            float bestCost = integration[open[0]];
            for (int i = 1; i < open.Count; i++)
            {
                float c = integration[open[i]];
                if (c < bestCost)
                {
                    bestCost = c;
                    bestIdx = i;
                }
            }

            int cellIndex = open[bestIdx];
            open.RemoveAt(bestIdx);

            int cx = cellIndex % gridWidth;
            int cy = cellIndex / gridWidth;

            float cellCost = integration[cellIndex];

            // expand neighbors in fixed order
            for (int n = 0; n < 8; n++)
            {
                int nx = cx + neighborDX[n];
                int ny = cy + neighborDY[n];

                if (nx < 0 || nx >= gridWidth || ny < 0 || ny >= gridHeight) continue;

                int nIndex = nx + ny * gridWidth;
                if (!walkable[nIndex]) continue;

                float newCost = cellCost + neighborCost[n];
                if (newCost < integration[nIndex])
                {
                    integration[nIndex] = newCost;
                    if (!open.Contains(nIndex)) open.Add(nIndex);
                }
            }
        }

        // compute flow vectors deterministically
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int idx = x + y * gridWidth;
                if (!walkable[idx] || float.IsPositiveInfinity(integration[idx]))
                {
                    flow[idx] = Vector2.zero;
                    continue;
                }

                // find neighbor with smallest integration
                float best = integration[idx];
                int bestNX = -1, bestNY = -1;

                for (int n = 0; n < 8; n++)
                {
                    int nx = x + neighborDX[n];
                    int ny = y + neighborDY[n];
                    if (nx < 0 || nx >= gridWidth || ny < 0 || ny >= gridHeight) continue;
                    int nIndex = nx + ny * gridWidth;
                    float val = integration[nIndex];
                    if (val < best)
                    {
                        best = val;
                        bestNX = nx;
                        bestNY = ny;
                    }
                }

                if (bestNX >= 0)
                {
                    Vector2 from = CellCenterWorld(x, y);
                    Vector2 to = CellCenterWorld(bestNX, bestNY);
                    flow[idx] = (to - from).normalized;
                }
                else
                {
                    flow[idx] = Vector2.zero;
                }
            }
        }
    }

    // Public query: returns a world-space normalized direction to follow
    public Vector2 GetDirection(Vector2 worldPos)
    {
        Vector2Int cell = WorldToCellInt(worldPos);
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
        {
            return Vector2.zero;
        }

        int idx = cell.x + cell.y * gridWidth;
        Vector2 dir = flow[idx];
        return dir;
    }

    // Helpers
    public Vector2 CellCenterWorld(int x, int y)
    {
        return new Vector2(
            worldOrigin.x + (x + 0.5f) * cellSize,
            worldOrigin.y + (y + 0.5f) * cellSize
        );
    }

    private Vector2Int WorldToCellInt(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - worldOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - worldOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    private Vector2 WorldToCell(Vector2 worldPos)
    {
        Vector2Int ci = WorldToCellInt(worldPos);
        return new Vector2(ci.x, ci.y);
    }

    // Optional debug accessor
    public bool IsWalkableCell(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return false;
        return walkable[x + y * gridWidth];
    }

    public Vector2 FlowAtCell(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return Vector2.zero;
        return flow[x + y * gridWidth];
    }
}
