using UnityEngine;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{
    public Transform actionPanel;
    public GameObject actionPrefab;

    public void AddVisualAction(Sprite sprite)
    {
        GameObject obj = Instantiate(actionPrefab, actionPanel);

        Image img = obj.GetComponent<Image>();

        if (img != null)
        {
            img.sprite = sprite;
        }
    }

    public void ClearVisualActions()
    {
        foreach (Transform child in actionPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void RemoveLastVisualAction()
    {
        if (actionPanel.childCount == 0)
            return;

        Transform lastChild = actionPanel.GetChild(actionPanel.childCount - 1);

        Destroy(lastChild.gameObject);
    }
}