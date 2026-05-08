using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{
    [System.Serializable]
    public class ActionSprite
    {
        public CircuitManager.ActionType action;
        public Sprite sprite;
    }

    [Header("UI")]
    public Transform actionPanel;
    public GameObject actionPrefab;

    [Header("Sprites")]
    public List<ActionSprite> actionSprites;

    Dictionary<CircuitManager.ActionType, Sprite> spriteDict = new();

    void Awake()
    {
        foreach (var entry in actionSprites)
        {
            spriteDict[entry.action] = entry.sprite;
        }
    }

    public void AddVisualAction(CircuitManager.ActionType action)
    {
        if (!spriteDict.ContainsKey(action))
            return;

        GameObject obj = Instantiate(actionPrefab, actionPanel);

        Image img = obj.GetComponent<Image>();

        if (img != null)
            img.sprite = spriteDict[action];
    }

    public void ClearVisualActions()
    {
        foreach (Transform child in actionPanel)
        {
            Destroy(child.gameObject);
        }
    }
}