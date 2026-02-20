using UnityEngine;

public class Store : MonoBehaviour
{
    [Tooltip("Tag del jugador que puede abrir el baúl")]
    public string playerTag = "Player";

    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;
    [Header("UI Store")]
    [SerializeField] private GameObject inventoryCanvas; // Asignar el InventoryCanvas del Store
    [SerializeField] private StoreUI storeUI; // Asignar el StoreUI

    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No se encontró Animator en el baúl.");
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (openSFX != null)
                audioSource.PlayOneShot(openSFX);
            animator?.SetTrigger("TransiteOpen");
            if (inventoryCanvas != null)
            {
                inventoryCanvas.SetActive(true);
                if (storeUI != null) storeUI.RefreshUI();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (closeSFX != null)
                audioSource.PlayOneShot(closeSFX);
            animator?.SetTrigger("TransiteClose");
            if (inventoryCanvas != null)
                inventoryCanvas.SetActive(false);
        }
    }
}
