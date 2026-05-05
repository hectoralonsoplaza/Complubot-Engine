using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GridPainter : MonoBehaviour
{
    [Header("References")]
    public Grid grid;
    public Tilemap tilemap;

    private TileBase selectedTile;

    // seleccionar
    public void SelectTile(TileBase tile)
    {
        selectedTile = tile;
    }

    
    void Update()
    {
        if (Mouse.current == null) return;

        // colocar
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            PlaceTile();
        }

        // borrar
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            DeleteTile();
        }
    }

    void PlaceTile()
    {
        if (selectedTile == null) return;

        TileButtons button = FindButton(selectedTile);

        // 🔴 BLOQUEO REAL
        if (button != null && !button.CanUse())
        {
            Debug.Log("No quedan usos para este tile");
            return;
        }

        Vector3 worldPos = GetMouseWorldPosition();
        Vector3Int cellPos = grid.WorldToCell(worldPos);

        tilemap.SetTile(cellPos, selectedTile);

        if (button != null)
            button.RegisterPlace();
    }

    void DeleteTile()
    {
        Vector3 worldPos = GetMouseWorldPosition();
        Vector3Int cellPos = grid.WorldToCell(worldPos);

        TileBase tile = tilemap.GetTile(cellPos);

        if (tile == null) return;

        tilemap.SetTile(cellPos, null);

        TileButtons button = FindButton(tile);

        if (button != null)
            button.RegisterRemove();
    }

    TileButtons FindButton(TileBase tile)
    {
        TileButtons[] buttons =
            Object.FindObjectsByType<TileButtons>(FindObjectsSortMode.None);

        foreach (var b in buttons)
        {
            if (b.tile == tile)
                return b;
        }

        return null;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
}