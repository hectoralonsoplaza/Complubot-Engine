using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GridPainter : MonoBehaviour
{
    [Header("References")]
    public Grid grid;
    public Tilemap tilemap;
    public Tilemap tilemap_logic;

    private TileBase draggingTile;
    private TileButtons.TileLayer draggingLayer;

    private Vector3Int currentCell;

    [Header("Ghost Preview")]
    public SpriteRenderer ghostRenderer;

    public void StartDrag(TileBase tile, TileButtons.TileLayer layer)
    {
        draggingTile = tile;
        draggingLayer = layer;

        UpdateGhost();
    }

    void Start()
    {
        if (ghostRenderer != null)
            ghostRenderer.enabled = false;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        bool overUI = EventSystem.current != null &&
                      EventSystem.current.IsPointerOverGameObject();

        Vector3 worldPos = GetMouseWorldPosition();
        currentCell = grid.WorldToCell(worldPos);

        HandleGhost();

        // -------------------------
        // DROP TILE (click izquierdo)
        // -------------------------
        if (Mouse.current.leftButton.wasReleasedThisFrame && !overUI)
        {
            TryPlace();
        }

        // -------------------------
        // DELETE TILE (click derecho)
        // -------------------------
        if (Mouse.current.rightButton.wasReleasedThisFrame && !overUI)
        {
            TryDelete();
        }
    }

    void HandleGhost()
    {
        if (ghostRenderer == null || draggingTile == null)
        {
            if (ghostRenderer != null)
                ghostRenderer.enabled = false;
            return;
        }

        ghostRenderer.enabled = true;

        ghostRenderer.transform.position = Vector3.Lerp(
            ghostRenderer.transform.position,
            grid.GetCellCenterWorld(currentCell),
            25f * Time.deltaTime
        );
    }

    void UpdateGhost()
    {
        if (ghostRenderer == null) return;

        if (draggingTile == null)
        {
            ghostRenderer.enabled = false;
            return;
        }

        ghostRenderer.enabled = true;

        if (draggingTile is Tile tile)
            ghostRenderer.sprite = tile.sprite;
    }

    void TryPlace()
    {
        if (draggingTile == null) return;

        TileButtons button = FindButton(draggingTile);
        if (button != null && !button.CanUse())
            return;

        Tilemap targetMap = (draggingLayer == TileButtons.TileLayer.Base)
            ? tilemap
            : tilemap_logic;

        targetMap.SetTile(currentCell, draggingTile);

        if (button != null)
            button.RegisterPlace();

        draggingTile = null;

        if (ghostRenderer != null)
            ghostRenderer.enabled = false;
    }

    void TryDelete()
    {
        // borrar primero lógica (prioridad arriba)
        TileBase logicTile = tilemap_logic.GetTile(currentCell);
        if (logicTile != null)
        {
            tilemap_logic.SetTile(currentCell, null);

            TileButtons button = FindButton(logicTile);
            if (button != null) button.RegisterRemove();
            return;
        }

        // luego base
        TileBase baseTile = tilemap.GetTile(currentCell);
        if (baseTile != null)
        {
            tilemap.SetTile(currentCell, null);

            TileButtons button = FindButton(baseTile);
            if (button != null) button.RegisterRemove();
        }
    }

    TileButtons FindButton(TileBase tile)
    {
        TileButtons[] buttons = Object.FindObjectsByType<TileButtons>(FindObjectsSortMode.None);

        foreach (var b in buttons)
            if (b.tile == tile) return b;

        return null;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }
}