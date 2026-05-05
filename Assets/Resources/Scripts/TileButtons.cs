using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TileButtons : MonoBehaviour
{
    [Header("Tiles")]
    public TileBase tile;

    [Header("Limites")]
    public int maxUses = 7;

    [HideInInspector]
    public int currentUses = 0;

    [Header("Referencias")]
    public GridPainter painter;
    public Button button;

    void Start()
    {
        button.onClick.AddListener(SelectTile);
        UpdateState();
    }

    public void SelectTile()
    {
        painter.SelectTile(tile);
    }

    // puede o no usarse
    public bool CanUse()
    {
        return currentUses < maxUses;
    }

    // registro de pintar
    public void RegisterPlace()
    {
        currentUses++;
        UpdateState();
    }

    // registro de borrar
    public void RegisterRemove()
    {
        currentUses = Mathf.Max(0, currentUses - 1);
        UpdateState();
    }

    // deactivar el boton
    void UpdateState()
    {
        if (button != null)
            button.interactable = CanUse();
    }
}