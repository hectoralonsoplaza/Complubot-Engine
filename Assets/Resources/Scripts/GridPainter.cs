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

    void Update()
    {
        if (Mouse.current == null) return;

        Vector3 worldPos = GetMouseWorldPosition();
        currentCell = grid.WorldToCell(worldPos);

        HandleGhost();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TryPlace();
        }
    }

    // 🔥 llamado desde UI al tocar el botón
    public void StartDrag(TileBase tile, TileButtons.TileLayer layer)
    {
        draggingTile = tile;
        draggingLayer = layer;

        UpdateGhostSprite();
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

    void UpdateGhostSprite()
    {
        if (ghostRenderer == null) return;

        if (draggingTile is Tile tile)
            ghostRenderer.sprite = tile.sprite;
    }

    void TryPlace()
    {
        if (draggingTile == null) return;

        TileButtons button = FindButton(draggingTile);
        if (button != null && !button.CanUse())
            return;

        Tilemap target = (draggingLayer == TileButtons.TileLayer.Base)
            ? tilemap
            : tilemap_logic;

        target.SetTile(currentCell, draggingTile);

        if (button != null)
            button.RegisterPlace();

        // 🔥 IMPORTANTE: NO seguimos arrastrando automáticamente
        draggingTile = null;

        if (ghostRenderer != null)
            ghostRenderer.enabled = false;
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