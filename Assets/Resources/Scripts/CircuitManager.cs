using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircuitManager : MonoBehaviour
{
    // acciones posibles
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

    public enum StartDirection
    {
        Left,
        Right
    }

    [System.Serializable]
    public class StartTileSet
    {
        public TileBase tile;
        public StartDirection direction;
    }

    [Header("Start Tiles")]
    public StartTileSet leftStartBlue;
    public StartTileSet leftStartPurple;
    public StartTileSet rightStartBlue;
    public StartTileSet rightStartPurple;

    [Header("Tilemaps")]
    public Tilemap logicMap;

    [Header("Tiles")]
    public List<TileBase> goalTiles;
    public List<TileBase> screwTiles;
    public List<TileBase> screwdriverTiles;
    public List<TileBase> forbiddenTiles;

    [Header("Robot")]
    public Transform child;
    public float stepDelay = 0.3f;

    Dictionary<Vector3Int, CellType> cells = new();
    Dictionary<ActionType, int> actions = new();

    // acciones en orden
    List<ActionType> actionOrder = new();

    Vector3Int startPos;
    Vector2Int startDir;

    Vector3Int robotPos;
    Vector2Int robotDir;

    bool hasScrew;
    bool hasDriver;

    bool isRunning;
    bool startReady;

    // inicio
    void Start()
    {
        StartCoroutine(CheckUntilStart());
    }

    IEnumerator CheckUntilStart()
    {
        while (true)
        {
            GenerateGridData();

            if (!startReady)
            {
                yield return new WaitForSeconds(0.3f);
                continue;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    // logica del grid
    void GenerateGridData()
    {
        cells.Clear();

        int startCount = 0;

        foreach (var pos in logicMap.cellBounds.allPositionsWithin)
        {
            if (!logicMap.HasTile(pos))
                continue;

            TileBase tile = logicMap.GetTile(pos);
            if (tile == null)
                continue;

            if (TryStart(tile, pos, leftStartBlue, ref startCount)) continue;
            if (TryStart(tile, pos, leftStartPurple, ref startCount)) continue;
            if (TryStart(tile, pos, rightStartBlue, ref startCount)) continue;
            if (TryStart(tile, pos, rightStartPurple, ref startCount)) continue;

            if (goalTiles.Contains(tile))
                cells[pos] = CellType.Goal;

            else if (screwTiles.Contains(tile))
                cells[pos] = CellType.Screw;

            else if (screwdriverTiles.Contains(tile))
                cells[pos] = CellType.Screwdriver;

            else if (forbiddenTiles.Contains(tile))
                cells[pos] = CellType.Forbidden;

            else
                cells[pos] = CellType.Empty;
        }

        if (startCount > 0)
        {
            if (!startReady)
            {
                Debug.Log("Start detectado");
                startReady = true;
                ResetRobot();
            }
        }
        else
        {
            if (startReady)
            {
                Debug.LogWarning("Start eliminado, buscando de nuevo...");
                startReady = false;
            }
        }
    }

    bool TryStart(TileBase tile, Vector3Int pos, StartTileSet set, ref int count)
    {
        if (tile == null || set.tile == null)
            return false;

        if (tile.name != set.tile.name)
            return false;

        count++;

        startPos = pos;
        startDir = (set.direction == StartDirection.Left)
            ? Vector2Int.left
            : Vector2Int.right;

        cells[pos] = CellType.Start;

        return true;
    }

    // añadir y borrar acciones
    public void AddAction(ActionType action)
    {
        if (!actions.ContainsKey(action))
            actions[action] = 0;

        actions[action]++;

        // guardar las acciones en orden
        actionOrder.Add(action);
    }

    public List<ActionType> GetActions()
    {
        return new List<ActionType>(actionOrder);
    }

    public void ClearActions()
    {
        actions.Clear();
        actionOrder.Clear();
    }

    // eliminar la ultima accon
    public void RemoveLastAction()
    {
        if (actionOrder.Count == 0)
            return;

        ActionType lastAction = actionOrder[actionOrder.Count - 1];

        actionOrder.RemoveAt(actionOrder.Count - 1);

        if (actions.ContainsKey(lastAction))
        {
            actions[lastAction]--;

            if (actions[lastAction] <= 0)
            {
                actions.Remove(lastAction);
            }
        }
    }

    List<ActionType> BuildActionList()
    {
        return new List<ActionType>(actionOrder);
    }

    // ejecucion de acciones
    public void Execute()
    {
        if (isRunning) return;

        if (!startReady)
        {
            Debug.Log("Aún no hay Start");
            return;
        }

        List<ActionType> executionList = BuildActionList();

        if (executionList.Count == 0 ||
            executionList[0] != ActionType.Start ||
            executionList[^1] != ActionType.End)
        {
            Debug.Log("Programa inválido");
            return;
        }

        ResetRobot();
        StartCoroutine(Run(executionList));
    }

    void ResetRobot()
    {
        robotPos = startPos;
        robotDir = startDir;

        hasScrew = false;
        hasDriver = false;

        if (child != null)
            child.rotation = Quaternion.identity;

        UpdateVisual();
    }

    IEnumerator Run(List<ActionType> executionList)
    {
        isRunning = true;

        foreach (var action in executionList)
        {
            yield return ExecuteAction(action);
        }

        isRunning = false;

        if (CheckLose())
            Debug.Log("Has perdido...");
        else if (CheckWin())
            Debug.Log("¡Has ganado!");
        else
            Debug.Log("No cumpliste los objetivos...");
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

    bool CheckLose()
    {
        if (!cells.ContainsKey(robotPos))
            return true;

        if (cells[robotPos] == CellType.Forbidden)
            return true;

        return false;
    }

    bool CheckWin()
    {
        if (!cells.ContainsKey(robotPos)) return false;
        if (cells[robotPos] != CellType.Goal) return false;

        return hasScrew && hasDriver;
    }

    void UpdateVisual()
    {
        if (child == null) return;

        child.position = logicMap.GetCellCenterWorld(robotPos);

        if (robotDir == Vector2Int.right)
            child.rotation = Quaternion.Euler(0, 0, 0);
        else if (robotDir == Vector2Int.left)
            child.rotation = Quaternion.Euler(0, 0, 180);
        else if (robotDir == Vector2Int.up)
            child.rotation = Quaternion.Euler(0, 0, 90);
        else if (robotDir == Vector2Int.down)
            child.rotation = Quaternion.Euler(0, 0, -90);
    }

    Vector3Int ToV3(Vector2Int v)
    {
        return new Vector3Int(v.x, v.y, 0);
    }
}