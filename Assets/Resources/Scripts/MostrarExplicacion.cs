using UnityEngine;

public class MostrarExplicacion : MonoBehaviour
{
    public GameObject panelExplicacion;
    private bool visible = false;

    public void ToggleExplicacion()
    {
        visible = !visible;
        panelExplicacion.SetActive(visible);
    }
}
