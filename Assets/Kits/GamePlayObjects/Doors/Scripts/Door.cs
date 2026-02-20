using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isOpen;
    [SerializeField] private bool isOpening;
    [SerializeField] private bool isClosing;

    [Header("Lock")]
    [SerializeField] private bool isLocked;
    [SerializeField] private string requiredKeyId;
    // ...existing code...

    // Detecta si el Player entra en el trigger de la puerta
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isLocked || isOpen || isOpening) return;

        // Solo reaccionar si el objeto es el Player
        var player = other.GetComponent<PlayerCharacter>();
        if (player == null) return;

        // Buscar el inventario del Player
        var inventory = player.GetComponent<Inventory>();
        if (inventory == null) return;

        // Si el Player tiene la llave, abrir la puerta automáticamente
        if (!string.IsNullOrEmpty(requiredKeyId) && inventory.HasItemWithId(requiredKeyId))
        {
            // Buscar el item llave en el inventario
            var allItems = inventory.GetAllItems();
            Item keyItem = null;
            foreach (var item in allItems)
            {
                if (item != null && item.ItemID == requiredKeyId)
                {
                    keyItem = item;
                    break;
                }
            }
            if (keyItem != null)
            {
                RequestOpen(requiredKeyId);
                inventory.TryRemoveItem(keyItem); // Eliminar la llave tras abrir
            }
        }
    }

    [Header("Animation")]
    // Trigger para abrir (transicion).
    [SerializeField] private string openTrigger = "TransitionOpen";
    // Trigger para cerrar (transicion).
    [SerializeField] private string closeTrigger = "TransitionClose";
    // Estado idle cuando esta abierta.
    [SerializeField] private string openIdleStateName = "OpenIdle";
    // Estado idle cuando esta cerrada.
    [SerializeField] private string closeIdleStateName = "CloseIdle";

    [Header("Audio")]
    // Sonido unico para abrir, cerrar y bloqueo.
    [SerializeField] private AudioClip doorSfx;
    // Sonido cuando se usa una llave y se desbloquea.
    [SerializeField] private AudioClip unlockSfx;

    [Header("Blocking")]
    // Collider que bloquea el paso cuando esta cerrada.
    private Collider2D blockingCollider;

    // Animator resuelto automaticamente.
    private Animator doorAnimator;
    // Fuente de audio detectada automaticamente.
    private AudioSource audioSource;

    public bool IsOpen => isOpen;
    public bool IsOpening => isOpening;
    public bool IsClosing => isClosing;
    public bool IsLocked => isLocked;
    public string RequiredKeyId => requiredKeyId;

    // Inicializacion principal de la puerta.
    private void Awake()
    {
        // Persistencia: si la puerta ya fue abierta, dejarla abierta
        if (PlayerPrefs.GetInt($"door_opened_{requiredKeyId}", 0) == 1)
        {
            isOpen = true;
            isLocked = false;
        }
        // Buscar AudioSource automaticamente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = GetComponentInChildren<AudioSource>();

        EnsureDoorAnimator();
        EnsureBlockingCollider();
        ApplyAnimatorState();
        ApplyBlockingState();
    }

    // En Start, el Animator ya esta listo y evitamos que se quede en el entry por defecto.
    private void Start()
    {
        ApplyAnimatorState();
        ApplyBlockingState();
    }

    // En editor, refresca estado para ver cambios sin entrar en play.
    private void OnValidate()
    {
        EnsureDoorAnimator();
        EnsureBlockingCollider();
        ApplyAnimatorState();
        ApplyBlockingState();
    }

    // Pide abrir la puerta, opcionalmente con id de llave.
    public bool RequestOpen(string keyId = null)
    {
        if (isOpen || isOpening) return true;

        // Si está cerrada con llave y la llave no coincide, no abrir ni sonar (o sonar locked si existe)
        if (isLocked && !HasMatchingKey(keyId))
        {
            if (unlockSfx != null)
                PlayOneShot(unlockSfx); // Usar unlockSfx como sonido de bloqueo si está asignado
            return false;
        }

        if (isLocked)
        {
            PlayOneShot(unlockSfx);
            isLocked = false;
        }

        isOpening = true;
        isClosing = false;

        // Dispara el trigger de abrir.
        if (doorAnimator != null && !string.IsNullOrWhiteSpace(openTrigger))
        {
            doorAnimator.SetTrigger(openTrigger);
        }
        // El sonido de abrir se reproduce solo al final de la animación (OnOpenFinished)
        return true;
    }

    // Pide cerrar la puerta (si estaba abierta).
    public bool RequestClose()
    {
        if (!isOpen || isClosing) return true;

        isClosing = true;
        isOpening = false;

        // Dispara el trigger de cerrar.
        if (doorAnimator != null && !string.IsNullOrWhiteSpace(closeTrigger))
        {
            doorAnimator.SetTrigger(closeTrigger);
        }

        PlayOneShot(doorSfx);
        return true;
    }

    // Forzar el estado de apertura sin animaciones.
    public void ForceSetOpen(bool open)
    {
        isOpen = open;
        isOpening = false;
        isClosing = false;

        ApplyAnimatorState();
        ApplyBlockingState();
    }

    // Comprueba si la llave coincide con la requerida.
    private bool HasMatchingKey(string keyId)
    {
        if (string.IsNullOrWhiteSpace(requiredKeyId)) return true;
        if (string.IsNullOrWhiteSpace(keyId)) return false;

        return string.Equals(requiredKeyId, keyId, System.StringComparison.OrdinalIgnoreCase);
    }

    // Llamar desde un Animation Event al final de la animacion de abrir.
    public void OnOpenFinished()
    {
        isOpen = true;
        isOpening = false;

        // Marcar la puerta como abierta en PlayerPrefs
        PlayerPrefs.SetInt($"door_opened_{requiredKeyId}", 1);
        PlayerPrefs.Save();

        ApplyAnimatorState();
        ApplyBlockingState();
        PlayOneShot(doorSfx); // Sonido de abrir puerta SOLO aquí
    }

    // Llamar desde un Animation Event al final de la animacion de cerrar.
    public void OnCloseFinished()
    {
        isOpen = false;
        isClosing = false;

        ApplyAnimatorState();
        ApplyBlockingState();
    }

    // Busca el Animator en este objeto.
    private void EnsureDoorAnimator()
    {
        if (doorAnimator != null) return;
        doorAnimator = GetComponent<Animator>();
    }

    // Busca el Collider2D en este objeto para bloquear el paso.
    private void EnsureBlockingCollider()
    {
        if (blockingCollider != null) return;

        Collider2D[] colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].isTrigger)
            {
                blockingCollider = colliders[i];
                return;
            }
        }
    }

    // Ajusta el estado idle del Animator segun si esta abierta o cerrada.
    private void ApplyAnimatorState()
    {
        if (doorAnimator == null) return;
        if (!doorAnimator.isActiveAndEnabled) return;
        if (doorAnimator.runtimeAnimatorController == null) return;

        // Estado segun el flag interno, para iniciar bien la escena.
        string stateName = isOpen ? openIdleStateName : closeIdleStateName;
        if (string.IsNullOrWhiteSpace(stateName)) return;

        doorAnimator.Play(stateName, 0, 0f);
    }

    // Activa o desactiva el collider que bloquea el paso.
    private void ApplyBlockingState()
    {
        if (blockingCollider == null) return;

        blockingCollider.enabled = !isOpen;
    }

    // Reproduce sonido si hay fuente y clip.
    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.PlayOneShot(clip, AudioManager.SfxVolume);
    }
}
