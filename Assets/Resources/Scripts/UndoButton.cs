using UnityEngine;

public class UndoButton : MonoBehaviour
{
    public CircuitManager manager;
    public ActionManager uiManager;

    public void UndoLastAction()
    {
        manager.RemoveLastAction();
        uiManager.RemoveLastVisualAction();
    }
}