using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircuitManager : MonoBehaviour
{
    // acciones

    public enum ActionType
    {
        Start,
        Forward,
        Backward,
        TurnRight,
        TurnLeft,
        Grab,
        Jump,
        End
    }

    // logica del grid

    public enum CellType
    {
        Empty,
        Screw,
        Screwdriver,
        Forbidden,
        ForbiddenColor,
        Goal,
        Start
    }

    public enum CellColor
    {
        None,
        Yellow,
        Orange,
        Red,
        Green
    }

    public enum StartDirection
    {
        Up,
        Down,
        Left,
        Right
    }


    [System.Serializable]
    public class StartTileSet
    {
        public TileBase blue;
        public TileBase purple;
        public StartDirection direction;
    }

    [Header("Start")]
    public StartTileSet up;
    public StartTileSet down;
    public StartTileSet left;
    public StartTileSet right;


    [Header("Tilemaps")]
    public Tilemap logicMap;


    [Header("Goal Tiles")]
    public List<TileBase> goalTiles;

    [Header("Objects")]
    public List<TileBase> screwTiles;
    public List<TileBase> screwdriverTiles;

    [Header("Forbidden")]
    public List<TileBase> forbiddenTiles;

    [Header("Forbidden Colors")]
    public List<TileBase> forbiddenYellowTiles;
    public List<TileBase> forbiddenOrangeTiles;
    public List<TileBase> forbiddenRedTiles;
    public List<TileBase> forbiddenGreenTiles;


    [Header("Robot")]
    public Transform robotVisual;
    public float stepDelay = 0.3f;

    // logica de datos

    Dictionary<Vector3Int, CellType> cells = new();
    Dictionary<Vector3Int, CellColor> colors = new();

    List<ActionType> actions = new();

    Vector3Int startPos;
    Vector2Int startDir;

    Vector3Int robotPos;
    Vector2Int robotDir;

    bool hasScrew;
    bool hasDriver;

    bool needsScrew;
    bool needsDriver;

    Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    void Start()
    {
        GenerateGridData();
    }

    // logica del circuito

    void GenerateGridData()
    {
        cells.Clear();
        colors.Clear();

        needsScrew = false;
        needsDriver = false;

        int startCount = 0;
        int goalCount = 0;

        foreach (var pos in logicMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = logicMap.GetTile(pos);
            if (tile == null) continue;

            // comienzo 
            if (TryStart(tile, pos, up, StartDirection.Up, ref startCount)) continue;
            if (TryStart(tile, pos, down, StartDirection.Down, ref startCount)) continue;
            if (TryStart(tile, pos, left, StartDirection.Left, ref startCount)) continue;
            if (TryStart(tile, pos, right, StartDirection.Right, ref startCount)) continue;

            // meta
            if (goalTiles.Contains(tile))
            {
                goalCount++;

                if (goalCount > 1)
                    Debug.LogError("❌ Hay más de un GOAL");

                cells[pos] = CellType.Goal;
            }

            // tornillo
            else if (screwTiles.Contains(tile))
            {
                cells[pos] = CellType.Screw;
                needsScrew = true;
            }
            // destornillador
            else if (screwdriverTiles.Contains(tile))
            {
                cells[pos] = CellType.Screwdriver;
                needsDriver = true;
            }

            // tiles prohibidos
            else if (forbiddenTiles.Contains(tile))
            {
                cells[pos] = CellType.Forbidden;
            }

            // colores
            else if (forbiddenYellowTiles.Contains(tile))
            {
                cells[pos] = CellType.ForbiddenColor;
                colors[pos] = CellColor.Yellow;
            }
            else if (forbiddenOrangeTiles.Contains(tile))
            {
                cells[pos] = CellType.ForbiddenColor;
                colors[pos] = CellColor.Orange;
            }
            else if (forbiddenRedTiles.Contains(tile))
            {
                cells[pos] = CellType.ForbiddenColor;
                colors[pos] = CellColor.Red;
            }
            else if (forbiddenGreenTiles.Contains(tile))
            {
                cells[pos] = CellType.ForbiddenColor;
                colors[pos] = CellColor.Green;
            }
        }

        if (startCount == 0)
            Debug.LogError("No hay comienzo en el circuito");

        if (goalCount == 0)
            Debug.LogError("No hay meta en el circuito");
    }

    bool TryStart(TileBase tile, Vector3Int pos, StartTileSet set, StartDirection dir, ref int count)
    {
        if (tile != set.blue && tile != set.purple)
            return false;

        count++;

        if (count > 1)
            Debug.LogError("Hay mas de un comienzo en el circuito");

        startPos = pos;
        startDir = DirectionToVector(dir);

        cells[pos] = CellType.Start;

        return true;
    }

    Vector2Int DirectionToVector(StartDirection dir)
    {
        switch (dir)
        {
            case StartDirection.Up: return Vector2Int.up;
            case StartDirection.Down: return Vector2Int.down;
            case StartDirection.Left: return Vector2Int.left;
            case StartDirection.Right: return Vector2Int.right;
        }

        return Vector2Int.up;
    }

    // botones

    public void AddAction(ActionType action)
    {
        actions.Add(action);
    }

    public void ClearActions()
    {
        actions.Clear();
    }

    public void Execute()
    {
        if (actions.Count == 0 ||
            actions[0] != ActionType.Start ||
            actions[^1] != ActionType.End)
        {
            
            return;
        }

        ResetRobot();
        StartCoroutine(Run());
    }

    void ResetRobot()
    {
        robotPos = startPos;
        robotDir = startDir;

        hasScrew = false;
        hasDriver = false;

        UpdateVisual();
    }

    // ejecucion del circuito

    IEnumerator Run()
    {
        foreach (var action in actions)
        {
            yield return ExecuteAction(action);

            if (CheckLose())
            {
                Debug.Log("Has perdido... ¡Sigue intentándolo!");
                yield break;
            }
        }

        if (CheckWin())
            Debug.Log("¡Has ganado!");
        else
            Debug.Log("No has cumplido los objetivos... ¡Sigue intentándolo!");
    }

    IEnumerator ExecuteAction(ActionType action)
    {
        switch (action)
        {
            case ActionType.Forward:
                robotPos += ToV3(robotDir);
                break;

            case ActionType.Backward:
                robotPos -= ToV3(robotDir);
                break;

            case ActionType.TurnRight:
                robotDir = new Vector2Int(robotDir.y, -robotDir.x);
                break;

            case ActionType.TurnLeft:
                robotDir = new Vector2Int(-robotDir.y, robotDir.x);
                break;

            case ActionType.Grab:
                TryGrab();
                break;

            case ActionType.Jump:
                robotPos += ToV3(robotDir) * 2;
                break;
        }

        UpdateVisual();
        yield return new WaitForSeconds(stepDelay);
    }

    void TryGrab()
    {
        if (!cells.ContainsKey(robotPos)) return;

        if (cells[robotPos] == CellType.Screw)
        {
            hasScrew = true;
            cells[robotPos] = CellType.Empty;
            logicMap.SetTile(robotPos, null);
        }

        if (cells[robotPos] == CellType.Screwdriver)
        {
            hasDriver = true;
            cells[robotPos] = CellType.Empty;
            logicMap.SetTile(robotPos, null);
        }
    }

    // logica de reglas

    bool CheckLose()
    {
        if (!cells.ContainsKey(robotPos))
            return true;

        var cell = cells[robotPos];

        if (cell == CellType.Forbidden)
            return true;

        if (cell == CellType.ForbiddenColor)
        {
            var color = colors[robotPos];

            foreach (var dir in directions)
            {
                var adj = robotPos + dir;

                if (colors.TryGetValue(adj, out var c))
                {
                    if (c == color)
                        return true;
                }
            }
        }

        return false;
    }

    bool CheckWin()
    {
        if (!cells.ContainsKey(robotPos)) return false;

        if (cells[robotPos] != CellType.Goal)
            return false;

        if (needsScrew && !hasScrew) return false;
        if (needsDriver && !hasDriver) return false;

        return true;
    }

    // visuales

    void UpdateVisual()
    {
        Vector3 world = logicMap.GetCellCenterWorld(robotPos);
        robotVisual.position = world;

        float angle = Mathf.Atan2(robotDir.y, robotDir.x) * Mathf.Rad2Deg;
        robotVisual.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    Vector3Int ToV3(Vector2Int v)
    {
        return new Vector3Int(v.x, v.y, 0);
    }
}