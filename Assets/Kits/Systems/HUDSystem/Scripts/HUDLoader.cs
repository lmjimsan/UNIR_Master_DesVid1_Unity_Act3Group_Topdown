using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDLoader : MonoBehaviour
{
    private void Start()
    {
        // Verificar si la escena HUD ya está cargada
        Scene hudScene = SceneManager.GetSceneByName("HUD");
        
        // Si no está cargada, cargarla de forma aditiva
        if (!hudScene.isLoaded)
        {
            SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
        }
    }
}
