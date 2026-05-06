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

    private TileBase selectedTile;
    private TileButtons.TileLayer selectedLayer;

    // seleccionar
    public void SelectTile(TileBase tile, TileButtons.TileLayer layer)
    {
        selectedTile = tile;
        selectedLayer = layer;
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

        if (button != null && !button.CanUse())
        {
            Debug.Log("No quedan usos para este tile");
            return;
        }

        Vector3 worldPos = GetMouseWorldPosition();
        Vector3Int cellPos = grid.WorldToCell(worldPos);

        Tilemap targetMap = (selectedLayer == TileButtons.TileLayer.Base)
            ? tilemap
            : tilemap_logic;

        targetMap.SetTile(cellPos, selectedTile);

        if (button != null)
            button.RegisterPlace();
    }

    void DeleteTile()
    {
        Vector3 worldPos = GetMouseWorldPosition();
        Vector3Int cellPos = grid.WorldToCell(worldPos);

        // borrar en orden de arriba a abajo
        TileBase logicTile = tilemap_logic.GetTile(cellPos);

        if (logicTile != null)
        {
            tilemap_logic.SetTile(cellPos, null);

            TileButtons button = FindButton(logicTile);
            if (button != null)
                button.RegisterRemove();

            return;
        }

        // si no hay nada arriba borrar abajo
        TileBase baseTile = tilemap.GetTile(cellPos);

        if (baseTile != null)
        {
            tilemap.SetTile(cellPos, null);

            TileButtons button = FindButton(baseTile);
            if (button != null)
                button.RegisterRemove();
        }
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