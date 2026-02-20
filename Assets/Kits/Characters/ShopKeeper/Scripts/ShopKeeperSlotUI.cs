using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopKeeperSlotUI : MonoBehaviour
{
    private PlayerInventoryUI playerInventoryUI;
    private Purse playerPurse;
    public int SlotIndex { get; private set; }
    private Image itemImage;
    private Item currentItem;
    private TMPro.TextMeshProUGUI priceText;

    private void Awake()
    {
        // Configurar EventTrigger para click izquierdo
        var trigger = GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
            trigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => OnPointerClick((UnityEngine.EventSystems.PointerEventData)data));
        trigger.triggers.Add(entry);
        // Buscar PlayerInventoryUI y Purse
        playerInventoryUI = FindFirstObjectByType<PlayerInventoryUI>();
        if (playerInventoryUI != null)
        {
            playerPurse = playerInventoryUI.GetComponentInParent<PlayerCharacter>()?.GetComponent<Purse>();
        }
        itemImage = GetComponent<Image>();
        // Extrae el sufijo numérico del nombre del slot (ej: Slot32)
        string slotName = gameObject.name;
        string digits = System.Text.RegularExpressions.Regex.Match(slotName, "\\d+").Value;
        if (int.TryParse(digits, out int idx))
            SlotIndex = idx - 1;
        else
            SlotIndex = 0;
        // Busca el TextMeshProUGUI hijo llamado TxtPriceSlotXX (donde XX es el mismo sufijo que el slot)
        var priceObj = transform.parent.Find($"TxtPriceSlot{digits}");
        if (priceObj != null)
            priceText = priceObj.GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        if (item != null)
        {
            itemImage.sprite = item.InventorySprite;
            itemImage.enabled = true;
            if (priceText != null)
            {
                int price = item.BuyPrice;
                priceText.text = price == -1 ? "N/C" : price.ToString();
            }
        }
        else
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            if (priceText != null)
                priceText.text = "";
        }
    }

    // Permite comprar el ítem al hacer clic izquierdo
    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (eventData.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left && currentItem != null)
        {
            if (playerInventoryUI == null)
                playerInventoryUI = FindFirstObjectByType<PlayerInventoryUI>();
            if (playerPurse == null && playerInventoryUI != null)
                playerPurse = playerInventoryUI.GetComponentInParent<PlayerCharacter>()?.GetComponent<Purse>();
            if (playerInventoryUI != null && playerPurse != null)
            {
                int price = currentItem.BuyPrice;
                if (price > 0 && playerPurse.CurrentCoins >= price)
                {
                    // Solo descuenta dinero si el ítem se añade al inventario
                    bool añadido = playerInventoryUI.GetInventory().TryAddItem(currentItem);
                    if (añadido)
                    {
                        playerPurse.OnGiveCoins(price);
                        playerInventoryUI.RefreshInventoryUI();
                        Debug.Log($"[ShopKeeperSlotUI] Compra realizada: {currentItem.ItemName} por {price} monedas.");
                    }
                    else
                    {
                        Debug.LogWarning($"[ShopKeeperSlotUI] Inventario lleno, no se puede comprar {currentItem.ItemName}.");
                    }
                }
                else
                {
                    Debug.LogWarning($"[ShopKeeperSlotUI] No tienes suficiente dinero para comprar {currentItem.ItemName} (precio: {price}, tienes: {playerPurse.CurrentCoins})");
                }
            }
        }
    }

    public Item GetItem() => currentItem;
    // Para que funcione el click, añade un EventTrigger en el prefab o desde ShopKeeperInventoryUI
}
