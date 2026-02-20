using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [Header("Canvas con la información del HUD")]
    [SerializeField] private GameObject hudInfoCanvas;

    private LifeBar lifeHUDBar;
    private PurseHUDAmount purseHUDAmount;

    private void Start()
    {
        // Obtener referencias de los componentes del canvas
        if (hudInfoCanvas != null)
        {
            lifeHUDBar = hudInfoCanvas.GetComponent<LifeBar>();
            purseHUDAmount = hudInfoCanvas.GetComponent<PurseHUDAmount>();
        }

        // Conectar al Player cuando se carga la escena HUD
        ConnectToPlayer();
        
        // Suscribirse al evento de carga de escena para reconectar cuando cambie
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reconectar al Player en la nueva escena
        ConnectToPlayer();
    }

    private void ConnectToPlayer()
    {
        // Buscar el Player en todas las escenas cargadas
        PlayerCharacter player = FindFirstObjectByType<PlayerCharacter>();
        if (player == null) return;

        // Conectar Life HUD Bar
        if (lifeHUDBar != null)
        {
            Life playerLife = player.GetComponent<Life>();
            if (playerLife != null)
            {
                lifeHUDBar.SetLifeOwner(playerLife);
            }
        }

        // Conectar Purse HUD Amount
        if (purseHUDAmount != null)
        {
            Purse playerPurse = player.GetComponent<Purse>();
            if (playerPurse != null)
            {
                purseHUDAmount.SetPurseOwner(playerPurse);
            }
        }
    }
}
