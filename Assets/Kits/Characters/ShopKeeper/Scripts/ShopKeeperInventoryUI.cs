using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopKeeperInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    private ShopKeeperSlotUI[] slotUIs;
    private Inventory shopKeeperInventory;

    private void Awake()
    {
        if (inventoryPanel == null)
            inventoryPanel = gameObject;
        slotUIs = GetComponentsInChildren<ShopKeeperSlotUI>(true);
        if (shopKeeperInventory == null)
            shopKeeperInventory = GetComponentInParent<Inventory>();
    }

    public void RefreshInventoryUI()
    {
        if (shopKeeperInventory == null) return;
        var items = shopKeeperInventory.GetAllItems();
        for (int i = 0; i < slotUIs.Length; i++)
        {
            Item item = (i < items.Count) ? items[i] : null;
            slotUIs[i].SetItem(item);
        }
    }

    public void ShowInventory()
    {
        Debug.Log($"[ShopKeeperInventoryUI] ShowInventory llamado. Activando panel: {inventoryPanel.name}", inventoryPanel);
        inventoryPanel.SetActive(true);
        RefreshInventoryUI();
    }

    public void HideInventory()
    {
        Debug.Log($"[ShopKeeperInventoryUI] HideInventory llamado. Desactivando panel: {inventoryPanel.name}", inventoryPanel);
        inventoryPanel.SetActive(false);
    }

    public Inventory GetInventory() => shopKeeperInventory;
}
