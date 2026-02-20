using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoreSlotUI : MonoBehaviour, IPointerClickHandler
{
    public int SlotIndex { get; private set; }
    private Image itemImage;
    private Item currentItem;
    private StoreUI storeUI;
    private PlayerInventoryUI playerInventoryUI;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        storeUI = GetComponentInParent<StoreUI>();
        playerInventoryUI = FindFirstObjectByType<PlayerInventoryUI>();
        string digits = System.Text.RegularExpressions.Regex.Match(gameObject.name, "\\d+").Value;
        if (int.TryParse(digits, out int idx))
            SlotIndex = idx - 11;
        else
            SlotIndex = 0;
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        if (item != null)
        {
            itemImage.sprite = item.InventorySprite;
            itemImage.enabled = true;
        }
        else
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
    }

    public Item GetItem() => currentItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log($"[StoreSlotUI] OnPointerClick en slot {SlotIndex} con item: {(currentItem != null ? currentItem.ItemName : "null")} (botón: {eventData.button})");
        if (eventData.button == PointerEventData.InputButton.Left && currentItem != null)
        {
            if (playerInventoryUI == null)
            {
                playerInventoryUI = FindFirstObjectByType<PlayerInventoryUI>();
                // Debug.Log($"[StoreSlotUI] playerInventoryUI se resolvió dinámicamente: {(playerInventoryUI != null ? "OK" : "null")}");
            }
            if (playerInventoryUI != null)
            {
                // Debug.Log($"[StoreSlotUI] Solicitando transferencia al Player desde slot {SlotIndex}");
                playerInventoryUI.TransferItemFromStoreToFirstEmptySlot(this);
                // Debug.Log($"[StoreSlotUI] TransferItemFromStoreToFirstEmptySlot llamado");
            }
            else
            {
                // Debug.LogWarning($"[StoreSlotUI] playerInventoryUI sigue siendo null en slot {SlotIndex}");
            }
        }
    }
}
