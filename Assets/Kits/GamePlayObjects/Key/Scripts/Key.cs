using UnityEngine;

public class KeyController : MonoBehaviour
{
    [Header("Key")]
    // ID unico de la llave para abrir puertas especificas.
    [SerializeField] private string keyId;
    // Referencia al ScriptableObject del item llave
    [SerializeField] private Item keyItem;

    [Header("Audio")]
    // Sonido al recoger la llave.
    [SerializeField] private AudioClip pickupSfx;

    // AudioSource detectada automaticamente
    private AudioSource audioSource;

    public string KeyId => keyId;
    public Item KeyItem => keyItem;
    public AudioClip PickupSfx => pickupSfx;
    public AudioSource AudioSource => audioSource;

    private void Awake()
    {
        // Persistencia: si la llave ya fue recogida, destruir el objeto
        if (PlayerPrefs.GetInt($"key_collected_{keyId}", 0) == 1)
        {
            Destroy(gameObject);
            return;
        }
        // Buscar AudioSource automaticamente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = GetComponentInChildren<AudioSource>();
    }

    // Detecta cuando el player toca la llave.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Añadir la llave al inventario del Player
        var inventory = other.GetComponent<Inventory>();
        if (inventory != null && keyItem != null)
        {
            if (inventory.TryAddItem(keyItem))
            {
                // Marcar la llave como recogida en PlayerPrefs
                PlayerPrefs.SetInt($"key_collected_{keyId}", 1);
                PlayerPrefs.Save();
                PickUp();
            }
        }
    }

    // Reproduce sonido y destruye la llave.
    private void PickUp()   
    {
        PlayPickupSfx();
        
        // Destruir visual pero mantener el GameObject hasta que termine el sonido
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        // Destruir después de que termine el sonido
        Destroy(gameObject, pickupSfx != null ? pickupSfx.length : 0.5f);
    }

    private void PlayPickupSfx()
    {
        if (pickupSfx == null) return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(pickupSfx, AudioManager.SfxVolume);
            return;
        }

        AudioSource.PlayClipAtPoint(pickupSfx, transform.position, AudioManager.SfxVolume);
    }
}
