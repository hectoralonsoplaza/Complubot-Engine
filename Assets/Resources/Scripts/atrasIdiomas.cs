using UnityEngine;
using UnityEngine.SceneManagement; // <- ¡No te olvides de esta línea!

public class CambiarEscena : MonoBehaviour
{
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}