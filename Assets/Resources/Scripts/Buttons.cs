using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public CircuitManager system;
    public CircuitManager.ActionType action;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            system.AddAction(action);
        });
    }
}