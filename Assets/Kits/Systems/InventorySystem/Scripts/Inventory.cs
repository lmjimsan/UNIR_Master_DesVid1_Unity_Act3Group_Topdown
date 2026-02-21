// --------------------------------------------------------------------------------------
// Inventory.cs
// Componente base para gestionar una colección de items (sin stacks).
// Añade este script a cualquier GameObject que necesite inventario: Player, baúl, Shopkeeper...
// Permite añadir, quitar, consultar y listar items.
// --------------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 20;
    [SerializeField] private List<Item> items = new List<Item>();

    // Eventos para UI o lógica externa
    public event System.Action<Item> OnItemAdded;
    public event System.Action<Item> OnItemRemoved;
    public event System.Action OnInventoryChanged;

    public bool TryAddItem(Item item)
    {
        if (items.Count >= maxSlots) return false;
        items.Add(item);
        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool TryRemoveItem(Item item)
    {
        if (!items.Contains(item)) return false;
        items.Remove(item);
        OnItemRemoved?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }

    public List<Item> GetAllItems()
    {
        return new List<Item>(items);
    }

    public int GetSlotCount()
    {
        return items.Count;
    }

    public int GetMaxSlots()
    {
        return maxSlots;
    }

    // Comprueba si el inventario contiene un item con un ID concreto
    public bool HasItemWithId(string itemId)
    {
        foreach (var item in items)
        {
            if (item != null && item.ItemID == itemId)
                return true;
        }
        return false;
    }
}
