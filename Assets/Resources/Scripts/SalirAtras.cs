using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class SalirAtras : MonoBehaviour
{
    // Método para cargar la escena del menú principal
    public void IrAlMenuPrincipal()
    {
        // "MainMenu" debe llamarse exactamente igual en tus carpetas
        SceneManager.LoadScene("MainMenu");
    }

   
}