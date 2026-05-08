using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public CircuitManager manager;
    public ActionManager uiManager;

    public CircuitManager.ActionType action;

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponentInChildren<Image>();

        GetComponent<Button>().onClick.AddListener(() =>
        {
            manager.AddAction(action);

            uiManager.AddVisualAction(buttonImage.sprite);
        });
    }
}