using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Script que gestiona la interacción del ShopKeeper.
/// Detecta cuando el jugador se acerca y activa el diálogo.
/// Requiere un Collider configurado como trigger en el mismo GameObject.
/// </summary>
public class ShopKeeper : MonoBehaviour
{
    [SerializeField] private ShopKeeperInventoryUI shopKeeperInventoryUI;
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private DialogueTypewriter dialogueTypewriter;
    [SerializeField] private Image nextButtonImage;
    [SerializeField] private string welcomeMessage = "¡Bienvenido a mi tienda!";

    // private bool isPlayerNear = false; // Campo eliminado, no se usa
    private Collider2D triggerCollider;

    private void Awake()
    {
        // Si no se asignó ShopKeeperInventoryUI, intentar encontrarlo como hijo
        if (shopKeeperInventoryUI == null)
        {
            shopKeeperInventoryUI = GetComponentInChildren<ShopKeeperInventoryUI>();
            if (shopKeeperInventoryUI == null)
            {
                Debug.LogError("[ShopKeeper] No se encontró ShopKeeperInventoryUI. Asigna la referencia en el Inspector.", gameObject);
            }
        }
        // Obtener el trigger collider
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider == null)
        {
            Debug.LogError("[ShopKeeper] No se encontró un Collider2D. Asegúrate de tener uno configurado como trigger.", gameObject);
        }
        // Si no se asignó el Canvas, intentar encontrarlo
        if (dialogueCanvas == null)
        {
            dialogueCanvas = GetComponentInChildren<Canvas>();
        }
        // Si no se asignó el DialogueTypewriter, intentar encontrarlo
        if (dialogueTypewriter == null)
        {
            dialogueTypewriter = GetComponentInChildren<DialogueTypewriter>();
        }
        // Si no se asignó la imagen del botón, intentar encontrarlo
        if (nextButtonImage == null)
        {
            nextButtonImage = GetComponentInChildren<Image>();
        }
        // Configurar el estado inicial del canvas
        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(false);
        }
        // Configurar la imagen como botón clickeable
        if (nextButtonImage != null)
        {
            // Buscar o crear un EventTrigger
            EventTrigger trigger = nextButtonImage.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = nextButtonImage.gameObject.AddComponent<EventTrigger>();
            }
            // Añadir listener para PointerClick
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => CloseDialogue());
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogWarning("[ShopKeeper] No se encontró la imagen nextButtonImage. El diálogo no se podrá cerrar con el botón.", gameObject);
        }
        // Suscribirse al evento de fin de tipeo
        if (dialogueTypewriter != null)
        {
            dialogueTypewriter.OnTypingComplete += OnDialogueTypingComplete;
        }
    }

    private void OnDialogueTypingComplete()
    {
        // Cerrar el diálogo y mostrar el inventario automáticamente
        CloseDialogue();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si es el jugador
        Debug.Log($"[ShopKeeper] OnTriggerEnter2D llamado por {collision.name} (tag={collision.tag})");
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerCharacter>() != null)
        {
            // isPlayerNear eliminado
            // Show both canvases immediately
            OpenDialogue();
            if (shopKeeperInventoryUI != null)
            {
                Debug.Log("[ShopKeeper] OnTriggerEnter2D: Llamando a ShowInventory del ShopKeeperInventoryUI");
                shopKeeperInventoryUI.ShowInventory();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verificar si es el jugador
        Debug.Log($"[ShopKeeper] OnTriggerExit2D llamado por {collision.name} (tag={collision.tag})");
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerCharacter>() != null)
        {
            // isPlayerNear eliminado
            CloseDialogue();
            // Ocultar inventario del ShopKeeper al salir del collider
            if (shopKeeperInventoryUI != null)
            {
                Debug.Log("[ShopKeeper] OnTriggerExit2D: Llamando a HideInventory del ShopKeeperInventoryUI");
                shopKeeperInventoryUI.HideInventory();
            }
        }
    }

    /// <summary>
    /// Abre el diálogo y comienza a mostrar el mensaje.
    /// </summary>
    public void OpenDialogue()
    {
        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(true);
        }

        if (dialogueTypewriter != null)
        {
            dialogueTypewriter.StartTyping(welcomeMessage);
        }
    }

    /// <summary>
    /// Cierra el diálogo y desactiva el canvas.
    /// </summary>
    public void CloseDialogue()
    {
        if (dialogueTypewriter != null)
        {
            dialogueTypewriter.StopTyping();
        }

        // Mostrar el inventario del ShopKeeper ANTES de ocultar el diálogo para evitar solapamientos
        if (shopKeeperInventoryUI != null)
        {
            Debug.Log("[ShopKeeper] CloseDialogue: Llamando a ShowInventory del ShopKeeperInventoryUI");
            shopKeeperInventoryUI.ShowInventory();
        }

        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Permite cambiar el mensaje de bienvenida.
    /// Útil si quieres mostrar diferentes mensajes.
    /// </summary>
    public void SetWelcomeMessage(string newMessage)
    {
        welcomeMessage = newMessage;
    }
}
