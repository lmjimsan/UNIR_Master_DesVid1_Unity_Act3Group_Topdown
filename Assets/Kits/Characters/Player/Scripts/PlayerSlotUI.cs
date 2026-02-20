using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerSlotUI : MonoBehaviour, IPointerClickHandler
{
    public int SlotIndex { get; private set; }
    private Image itemImage;
    private Item currentItem;
    private PlayerInventoryUI playerInventoryUI;
    private StoreUI storeUI;
    private TMPro.TextMeshProUGUI priceText;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        playerInventoryUI = GetComponentInParent<PlayerInventoryUI>();
        storeUI = FindFirstObjectByType<StoreUI>();
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

    public Item GetItem() => currentItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Evitar disparo del Player si el cursor está sobre UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Consumir item si su UseType es Manual con botón derecho
            if (eventData.button == PointerEventData.InputButton.Right && currentItem != null)
            {
                bool storeOpen = storeUI != null && storeUI.gameObject.activeInHierarchy;
                if (!storeOpen)
                {
                    if (currentItem.UseType == UseType.Manual)
                    {
                        var player = FindFirstObjectByType<PlayerCharacter>();
                        var dropDef = currentItem.GetType().GetProperty("dropDefinition")?.GetValue(currentItem) as DropDefinition;
                        if (dropDef != null && player != null)
                        {
                            if (dropDef.healthRecovery > 0 && player.Life != null)
                                player.Life.RecoverHealth(dropDef.healthRecovery);
                            if (dropDef.powerUpDamage > 0)
                                player.damage += dropDef.powerUpDamage;
                            if (dropDef.powerUpShield > 0)
                                player.shield += dropDef.powerUpShield;
                            if (dropDef.powerUpDuration > 0 && (dropDef.powerUpDamage > 0 || dropDef.powerUpShield > 0))
                                player.StartCoroutine(RevertPowerUp(player, dropDef.powerUpDamage, dropDef.powerUpShield, dropDef.powerUpDuration));
                        }
                        playerInventoryUI.GetInventory().TryRemoveItem(currentItem);
                        playerInventoryUI.RefreshInventoryUI();
                        Debug.Log($"Consumido: {currentItem.ItemName}");
                    }
                }
                else if (storeUI != null)
                {
                    storeUI.TransferItemFromPlayerToFirstEmptySlot(this);
                }
            }
        }
        {
            bool storeOpen = storeUI != null && storeUI.gameObject.activeInHierarchy;
            if (!storeOpen)
            {
                if (currentItem.UseType == UseType.Manual)
                {
                    var player = FindFirstObjectByType<PlayerCharacter>();
                    // Acceder a DropDefinition
                    var dropDef = currentItem.GetType().GetProperty("dropDefinition")?.GetValue(currentItem) as DropDefinition;
                    if (dropDef != null && player != null)
                    {
                        // Salud
                        if (dropDef.healthRecovery > 0 && player.Life != null)
                            player.Life.RecoverHealth(dropDef.healthRecovery);
                        // Power: daño y escudo
                        if (dropDef.powerUpDamage > 0)
                            player.damage += dropDef.powerUpDamage;
                        if (dropDef.powerUpShield > 0)
                            player.shield += dropDef.powerUpShield;
                        // Si hay duración, revertir tras ese tiempo
                        if (dropDef.powerUpDuration > 0 && (dropDef.powerUpDamage > 0 || dropDef.powerUpShield > 0))
                            player.StartCoroutine(RevertPowerUp(player, dropDef.powerUpDamage, dropDef.powerUpShield, dropDef.powerUpDuration));
                    }
                    // Consumir el ítem (eliminar del inventario)
                    playerInventoryUI.GetInventory().TryRemoveItem(currentItem);
                    playerInventoryUI.RefreshInventoryUI();
                    Debug.Log($"Consumido: {currentItem.ItemName}");
                }
            }
            else if (storeUI != null)
            {
                storeUI.TransferItemFromPlayerToFirstEmptySlot(this);
            }
        }
    }

    // Corrutina para revertir power up
    private System.Collections.IEnumerator RevertPowerUp(PlayerCharacter player, float damage, float shield, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (damage > 0) player.damage -= damage;
        if (shield > 0) player.shield -= shield;
        Debug.Log($"PowerUp revertido: -{damage} damage, -{shield} shield");
    }
}