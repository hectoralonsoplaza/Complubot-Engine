using UnityEngine;

public class UI : MonoBehaviour
{
    [Header("Objects")]
    public GameObject actionPanel;
    public GameObject child;

    bool opened = false;

    public void ToggleProgramming()
    {
        opened = !opened;

        actionPanel.SetActive(opened);
        child.SetActive(opened);
    }
}