using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar escenas

public class SceneChanger : MonoBehaviour
{
    // Esta función se llamará cuando presiones el botón
    public void CambiarAFloorCode()
    {
        // "FloorCode" debe coincidir exactamente con el nombre de tu escena
        SceneManager.LoadScene("FloorCode");
    }
}