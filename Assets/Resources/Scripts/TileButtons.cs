using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TileButtons : MonoBehaviour
{
    public enum TileLayer
    {
        Base,
        Logic
    }

    [Header("Tiles")]
    public TileBase tile;

    [Header("Capa")]
    public TileLayer layer;

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
        painter.SelectTile(tile, layer);
    }

    public bool CanUse()
    {
        return currentUses < maxUses;
    }

    public void RegisterPlace()
    {
        currentUses++;
        UpdateState();
    }

    public void RegisterRemove()
    {
        currentUses = Mathf.Max(0, currentUses - 1);
        UpdateState();
    }

    void UpdateState()
    {
        if (button != null)
            button.interactable = CanUse();
    }
}