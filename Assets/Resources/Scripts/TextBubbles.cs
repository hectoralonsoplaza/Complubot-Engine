using UnityEngine;

public class TextBubbles : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void LateUpdate()
    {
        if (!target) return;

        transform.position = target.position + offset;
    }
}