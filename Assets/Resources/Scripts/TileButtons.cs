using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileButtons : MonoBehaviour, IPointerDownHandler
{
    public enum TileLayer { Base, Logic }

    [Header("Tiles")]
    public TileBase tile;

    [Header("Layer")]
    public TileLayer layer;

    [Header("Limits")]
    public int maxUses = 7;
    [HideInInspector] public int currentUses = 0;

    [Header("References")]
    public GridPainter painter;
    public Button button;

    void Start()
    {
        UpdateState();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        painter.StartDrag(tile, layer);
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