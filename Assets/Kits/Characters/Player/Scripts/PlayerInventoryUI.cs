
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// PlayerInventoryUI gestiona la ventana de inventario del jugador.
/// Asigna en el inspector:
/// - inventoryPanel: el panel principal del inventario (GameObject)
/// - escButton: botón de cerrar (arriba a la derecha)
/// - quitButton: botón de cerrar (abajo)
/// - slotButtons: los 6 botones/huecos de objetos
/// - playerInventory: referencia al Inventory del Player
/// </summary>
public class PlayerInventoryUI : MonoBehaviour
{
    [Header("Sonidos de nivel (para todos los slots)")]
    [SerializeField] private AudioClip levelUp1to2Sfx;
    [SerializeField] private AudioClip levelUp2to3Sfx;
    [SerializeField] private AudioClip levelDownSfx;
    [SerializeField] private AudioSource playerAudioSource;

    public void PlayLevelChangeSound(int prevLevel, int newLevel)
    {
        if (playerAudioSource == null) return;
        if (newLevel > prevLevel)
        {
            if (prevLevel == 1 && newLevel == 2 && levelUp1to2Sfx != null)
                playerAudioSource.PlayOneShot(levelUp1to2Sfx);
            else if (prevLevel == 2 && newLevel == 3 && levelUp2to3Sfx != null)
                playerAudioSource.PlayOneShot(levelUp2to3Sfx);
        }
        else if (newLevel < prevLevel && levelDownSfx != null)
        {
            playerAudioSource.PlayOneShot(levelDownSfx);
        }
    }
    [Header("Referencias UI")]
    [SerializeField] private GameObject inventoryPanel;
    [Header("Botones de cerrar (como imágenes)")]
    [SerializeField] private GameObject escButtonImageObj;
    [SerializeField] private GameObject quitButtonImageObj;
    [SerializeField] private Sprite escNormalSprite;
    [SerializeField] private Sprite escPressedSprite;
    [SerializeField] private Sprite quitNormalSprite;
    [SerializeField] private Sprite quitPressedSprite;
    [SerializeField] private AudioClip closeButtonSfx;

    private PlayerSlotUI[] slotUIs;
    private Inventory playerInventory; // Siempre se busca en el padre
    private PlayerSlotUI draggingSlot;
    public Inventory GetInventory() => playerInventory;

    private void Awake()
    {
        // Buscar siempre el Inventory en el padre
        playerInventory = GetComponentInParent<Inventory>();
        // Buscar todos los PlayerSlotUI hijos y ordenarlos por SlotIndex
        slotUIs = GetComponentsInChildren<PlayerSlotUI>();
        System.Array.Sort(slotUIs, (a, b) => a.SlotIndex.CompareTo(b.SlotIndex));
        // Configurar cierre con imágenes (Esc y Quit)
        SetupCloseButton(escButtonImageObj, escNormalSprite, escPressedSprite);
        SetupCloseButton(quitButtonImageObj, quitNormalSprite, quitPressedSprite);
    }

    // Muestra el inventario del jugador
    public void ShowInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            RefreshInventoryUI();
        }
    }

    // Oculta el inventario del jugador
    public void HideInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    // Configura los botones de cerrar (Esc y Quit)
    private void SetupCloseButton(GameObject buttonObj, Sprite normal, Sprite pressed)
    {
        if (buttonObj == null) return;
        Image img = buttonObj.GetComponent<Image>();
        if (img != null && normal != null)
            img.sprite = normal;

        EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = buttonObj.AddComponent<EventTrigger>();

        // PointerDown: cambiar sprite a "pressed" y reproducir sonido
        EventTrigger.Entry downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        downEntry.callback.AddListener((data) => {
            if (img != null && pressed != null) img.sprite = pressed;
            if (playerAudioSource != null && closeButtonSfx != null)
                playerAudioSource.PlayOneShot(closeButtonSfx);
        });
        trigger.triggers.Add(downEntry);

        // PointerUp: volver a sprite normal y cerrar inventario
        EventTrigger.Entry upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        upEntry.callback.AddListener((data) => {
            if (img != null && normal != null) img.sprite = normal;
            HideInventory();
        });
        trigger.triggers.Add(upEntry);
    }

    public void RefreshInventoryUI()
    {
        if (playerInventory == null) return;
        var items = playerInventory.GetAllItems();
        for (int i = 0; i < slotUIs.Length; i++)
        {
            Item item = (i < items.Count) ? items[i] : null;
            slotUIs[i].SetItem(item);
        }
    }

    // Drag & Drop
    public void OnBeginDragSlot(PlayerSlotUI slot)
    {
        draggingSlot = slot;
    }
    public void OnEndDragSlot(PlayerSlotUI slot)
    {
        draggingSlot = null;
    }
    public void OnDropSlot(PlayerSlotUI targetSlot)
    {
        if (draggingSlot == null || targetSlot == null) return;
        if (targetSlot.GetItem() != null) return;
        Item item = draggingSlot.GetItem();
        if (item == null) return;
        if (playerInventory.TryRemoveItem(item))
        {
            playerInventory.TryAddItem(item);
            RefreshInventoryUI();
        }
    }

    // Transferir desde StoreSlotUI al primer slot vacío del Player
    public void TransferItemFromStoreToFirstEmptySlot(StoreSlotUI storeSlot)
    {
        Debug.Log($"[PlayerInventoryUI] TransferItemFromStoreToFirstEmptySlot llamado con storeSlot={(storeSlot != null ? storeSlot.name : "null")}");
        if (storeSlot == null) { /* Debug.LogWarning("[PlayerInventoryUI] storeSlot es null"); */ return; }
        Item item = storeSlot.GetItem();
        if (item == null) { /* Debug.LogWarning("[PlayerInventoryUI] El slot del Store no tiene item"); */ return; }
        Debug.Log($"[PlayerInventoryUI] Intentando transferir item: {item.ItemName}");
        bool transferido = false;
        foreach (var slot in slotUIs)
        {
            if (slot.GetItem() == null)
            {
                Inventory storeInventory = storeSlot.GetComponentInParent<StoreUI>()?.GetInventory();
                if (storeInventory == null) { /* Debug.LogWarning("[PlayerInventoryUI] No se encontró storeInventory"); */ break; }
                if (playerInventory == null) { /* Debug.LogWarning("[PlayerInventoryUI] No se encontró playerInventory"); */ break; }
                if (storeInventory.TryRemoveItem(item))
                {
                    Debug.Log($"[PlayerInventoryUI] Item {item.ItemName} removido del Store. Añadiendo al Player...");
                    if (playerInventory.TryAddItem(item))
                    {
                        Debug.Log($"[PlayerInventoryUI] Item {item.ItemName} añadido al Player correctamente");
                        RefreshInventoryUI();
                        FindFirstObjectByType<StoreUI>()?.RefreshUI();
                        transferido = true;
                    }
                    else
                    {
                        /* Debug.LogWarning($"[PlayerInventoryUI] No se pudo añadir el item {item.ItemName} al Player (inventario lleno?)"); */
                        // Si no se pudo añadir, devolver el item al Store
                        storeInventory.TryAddItem(item);
                    }
                }
                else
                {
                    Debug.LogWarning($"[PlayerInventoryUI] No se pudo remover el item {item.ItemName} del Store (no existe?)");
                }
                break;
            }
        }
        if (!transferido)
        {
            Debug.LogWarning("[PlayerInventoryUI] No se encontró slot vacío en el inventario del Player o la transferencia falló");
        }
    }

    // Transferencia desde Store al Player
    public void TransferItemFromStoreToPlayer(StoreSlotUI storeSlot, PlayerSlotUI playerSlot)
    {
        if (playerSlot.GetItem() != null) return;
        Item item = storeSlot.GetItem();
        if (item == null) return;
        Inventory storeInventory = storeSlot.GetComponentInParent<StoreUI>()?.GetInventory();
        if (storeInventory == null) return;
        if (storeInventory.TryRemoveItem(item))
        {
            playerInventory.TryAddItem(item);
            RefreshInventoryUI();
            FindFirstObjectByType<StoreUI>()?.RefreshUI();
        }
    }

    private void OnSlotClick(PointerEventData data, int slotIndex)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            if (playerInventory == null) return;
            var items = playerInventory.GetAllItems();
            if (slotIndex < items.Count && items[slotIndex] != null)
            {
                Item item = items[slotIndex];
                if (item.Type == ItemType.Consumable && item.UseType == UseType.Manual)
                {
                    // Aquí iría la lógica para consumir el objeto (ej: curar vida)
                    // playerInventory.TryRemoveItem(item);
                    Debug.Log($"Consumido: {item.ItemName}");
                }
            }
        }
    }


    // Llama a este método desde PlayerCharacter para cerrar el inventario desde código si lo necesitas
    public void ForceCloseInventory()
    {
        HideInventory();
    }
}
