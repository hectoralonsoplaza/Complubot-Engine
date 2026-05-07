using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
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