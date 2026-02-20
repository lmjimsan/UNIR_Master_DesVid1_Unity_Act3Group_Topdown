using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] public DropDefinition dropDefinition;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    internal void NotifyPickedUp()
    {
        // Reproducir sonido si existe
        if (pickupSfx != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(pickupSfx, AudioManager.SfxVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position, AudioManager.SfxVolume);
            }
        }
        
        // Destruir visual pero mantener el GameObject hasta que termine el sonido
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        // Destruir después de que termine el sonido
        Destroy(gameObject, pickupSfx != null ? pickupSfx.length : 0.5f);
    }
}
