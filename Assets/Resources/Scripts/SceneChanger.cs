using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Función para el botón Jugar
    public void CambiarAFloorCode()
    {
        SceneManager.LoadScene("FloorCode");
    }

    // Nueva función para el botón Instrucciones
    public void CambiarAInstrucciones()
    {
        SceneManager.LoadScene("Instructions");

    }
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CambiarAEjemplos()
    {
        SceneManager.LoadScene("Ejemplos");

    }

    public void CambiarAFloorCodeANIM()
    {
        SceneManager.LoadScene("FloorCodeANIM");

    }


}