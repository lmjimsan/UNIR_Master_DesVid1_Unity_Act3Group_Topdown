        using UnityEngine;
        using UnityEngine.UI;
        using System.Collections.Generic;

        public class StoreUI : MonoBehaviour
        {
            private StoreSlotUI[] slotUIs;
            private Inventory storeInventory;
            private StoreSlotUI draggingSlot;

            private void Awake()
            {
                slotUIs = GetComponentsInChildren<StoreSlotUI>();
                System.Array.Sort(slotUIs, (a, b) => a.SlotIndex.CompareTo(b.SlotIndex));
                storeInventory = GetComponentInParent<Inventory>();
            }

            private void OnEnable()
            {
                RefreshUI();
            }

            public void RefreshUI()
            {
                if (storeInventory == null) return;
                var items = storeInventory.GetAllItems();
                for (int i = 0; i < slotUIs.Length; i++)
                {
                    Item item = (i < items.Count) ? items[i] : null;
                    slotUIs[i].SetItem(item);
                }
            }

            public void OnBeginDragSlot(StoreSlotUI slot)
            {
                draggingSlot = slot;
            }
            public void OnEndDragSlot(StoreSlotUI slot)
            {
                draggingSlot = null;
            }
            public void OnDropSlot(StoreSlotUI targetSlot)
            {
                if (draggingSlot == null || targetSlot == null) return;
                if (targetSlot.GetItem() != null) return;
                Item item = draggingSlot.GetItem();
                if (item == null) return;
                if (storeInventory.TryRemoveItem(item))
                {
                    storeInventory.TryAddItem(item);
                    RefreshUI();
                }
            }

            // Transferir desde PlayerSlotUI al primer slot vacío del Store
            public void TransferItemFromPlayerToFirstEmptySlot(PlayerSlotUI playerSlot)
            {
                if (playerSlot == null || playerSlot.GetItem() == null) return;
                Item item = playerSlot.GetItem();
                foreach (var slot in slotUIs)
                {
                    if (slot.GetItem() == null)
                    {
                        // Transferencia real
                        Inventory playerInventory = playerSlot.GetComponentInParent<PlayerInventoryUI>()?.GetInventory();
                        if (playerInventory != null && storeInventory != null)
                        {
                            if (playerInventory.TryRemoveItem(item))
                            {
                                storeInventory.TryAddItem(item);
                                RefreshUI();
                                FindFirstObjectByType<PlayerInventoryUI>()?.RefreshInventoryUI();
                            }
                        }
                        break;
                    }
                }
            }

            // Transferencia desde Player al Store (slot a slot)
            public void TransferItemFromPlayerToStore(PlayerSlotUI playerSlot, StoreSlotUI storeSlot)
            {
                if (storeSlot.GetItem() != null) return;
                Item item = playerSlot.GetItem();
                if (item == null) return;
                Inventory playerInventory = playerSlot.GetComponentInParent<PlayerInventoryUI>()?.GetInventory();
                if (playerInventory == null) return;
                if (playerInventory.TryRemoveItem(item))
                {
                    storeInventory.TryAddItem(item);
                    RefreshUI();
                    FindFirstObjectByType<PlayerInventoryUI>()?.RefreshInventoryUI();
                }
            }

            public Inventory GetInventory()
            {
                return storeInventory;
            }
        }
