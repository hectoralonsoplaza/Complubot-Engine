using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public CircuitManager manager;
    public ActionManager uiManager;

    public CircuitManager.ActionType action;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            manager.AddAction(action);

            uiManager.AddVisualAction(action);
        });
    }
}