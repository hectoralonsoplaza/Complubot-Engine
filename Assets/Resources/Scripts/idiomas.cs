using UnityEngine;
using UnityEngine.SceneManagement; // Esencial para cambiar de escena

public class idiomas : MonoBehaviour
{
    // Este método lo llamará el botón
    public void CambiarAIdiomas()
    {
        // "Idiomas" debe llamarse exactamente igual que tu escena
        SceneManager.LoadScene("Idiomas");
    }
}