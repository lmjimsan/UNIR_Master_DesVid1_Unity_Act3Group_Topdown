using UnityEngine;

public class BottlePower : MonoBehaviour, IVisible2D
{
    [Header("Item asociado")]
    [SerializeField] private Item bottlePowerItem;
    public Item BottlePowerItem => bottlePowerItem;
    [SerializeField] int priority = 0;
    [SerializeField] IVisible2D.Side side = IVisible2D.Side.Neutrals;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    private AudioSource audioSource;

    int IVisible2D.GetPriority()
    {
        return priority;
    }

    IVisible2D.Side IVisible2D.GetSide()
    {
        return side;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Destruir visual pero mantener el GameObject hasta que termine el sonido
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        // Destruir después de un breve delay
        Destroy(gameObject, 0.5f);
    }

    private void PlayPickupSfx()
    {
        // Eliminado: el audio de recogida se gestiona en Drop
    }
}
