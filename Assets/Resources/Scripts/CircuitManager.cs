using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircuitManager : MonoBehaviour
{
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

    public enum CellColor
    {
        None,
        Yellow,
        Red,
        Blue
    }

    [Header("Tilemaps")]
    public Tilemap baseMap;
    public Tilemap logicMap;

    [Header("Tiles de referencia")]
    public TileBase startTile;
    public TileBase goalTile;
    public TileBase screwTile;
    public TileBase screwdriverTile;
    public TileBase forbiddenTile;

    public TileBase forbiddenYellow;
    public TileBase forbiddenRed;
    public TileBase forbiddenBlue;

    [Header("Robot")]
    public Transform robotVisual;
    public float moveSpeed = 5f;

    private Dictionary<Vector3Int, CellType> cells = new();
    private Dictionary<Vector3Int, CellColor> colors = new();

    private List<ActionType> actions = new();

    private Vector3Int startPos;
    private Vector2Int startDir;

    private Vector3Int robotPos;
    private Vector2Int robotDir;

    private bool hasScrew;
    private bool hasDriver;

    private bool needsScrew;
    private bool needsDriver;

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

    // grid

    void GenerateGridData()
    {
        cells.Clear();
        colors.Clear();

        foreach (var pos in logicMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = logicMap.GetTile(pos);
            if (tile == null) continue;

            if (tile == startTile)
            {
                cells[pos] = CellType.Start;
                startPos = pos;

                // direccion del sprite segun su rotacion
                startDir = Vector2Int.up;
            }
            else if (tile == goalTile)
                cells[pos] = CellType.Goal;

            else if (tile == screwTile)
            {
                cells[pos] = CellType.Screw;
                needsScrew = true;
            }
            else if (tile == screwdriverTile)
            {
                cells[pos] = CellType.Screwdriver;
                needsDriver = true;
            }
            else if (tile == forbiddenTile)
                cells[pos] = CellType.Forbidden;

            else if (tile == forbiddenYellow)
            {
                cells[pos] = CellType.ForbiddenColor;
                colors[pos] = CellColor.Yellow;
            }
            else if (tile == forbiddenRed)
            {
                cells[pos] = CellType.ForbiddenColor;
                colors[pos] = CellColor.Red;
            }
            else if (tile == forbiddenBlue)
            {
                cells[pos] = CellType.ForbiddenColor;
                colors[pos] = CellColor.Blue;
            }
        }
    }

    // botones de accion

    public void AddAction(ActionType action)
    {
        actions.Add(action);
        Debug.Log("Añadido: " + action);
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
            Debug.Log("Programa inválido");
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
                Debug.Log("¡Has perdido!");
                yield break;
            }
        }

        if (CheckWin())
            Debug.Log("¡Has ganado!");
        else
            Debug.Log("No lo conseguiste... ¡Sigue así!");
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
        yield return new WaitForSeconds(0.3f);
    }

    void TryGrab()
    {
        if (!cells.ContainsKey(robotPos)) return;

        if (cells[robotPos] == CellType.Screw)
            hasScrew = true;

        if (cells[robotPos] == CellType.Screwdriver)
            hasDriver = true;
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

    

    void UpdateVisual()
    {
        Vector3 world = logicMap.GetCellCenterWorld(robotPos);
        robotVisual.position = Vector3.Lerp(robotVisual.position, world, 1f);

        float angle = Mathf.Atan2(robotDir.y, robotDir.x) * Mathf.Rad2Deg;
        robotVisual.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    Vector3Int ToV3(Vector2Int v)
    {
        return new Vector3Int(v.x, v.y, 0);
    }
}