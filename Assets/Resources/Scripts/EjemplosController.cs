using UnityEngine;
using UnityEngine.SceneManagement;

public class EjemplosController : MonoBehaviour
{
    public void CargarEjemplo1()
    {
        SceneManager.LoadScene("Ejemplo1");
    }

    public void CargarEjemplo2()
    {
        SceneManager.LoadScene("Ejemplo2");
    }

    public void CargarEjemplo3()
    {
        SceneManager.LoadScene("Ejemplo3");
    }
}
