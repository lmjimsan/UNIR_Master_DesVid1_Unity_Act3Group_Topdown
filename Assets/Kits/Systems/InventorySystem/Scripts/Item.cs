// --------------------------------------------------------------------------------------
// Item.cs
// ScriptableObject que define los elementos interactuables del juego.
// Sirve para representar cualquier objeto que pueda estar en el inventario del jugador,
// en el baúl de la Home o en el almacén del Shopkeeper.
// Permite configurar: nombre, descripción, sprites, prefab, precios, tipo de uso y stack.
// Cada sistema (inventario, baúl, tienda) usará instancias de estos items para gestionar
// qué tiene cada uno y cómo se pueden mover, usar o intercambiar.
// --------------------------------------------------------------------------------------
using UnityEngine;

public enum ItemType
{
    Utility,        // Objetos útiles para el entorno (llaves, herramientas, etc.)
    Consumable,     // Objetos consumibles por el Player (pociones, comida, etc.)
    Equipment,      // Objetos que puede equiparse el Player (armaduras, armas, etc.)
    Miscellaneous   // Otros objetos que no encajan en los anteriores
}

public enum UseType
{
    Automatic, // Uso automático: si estamos cerca de algo que lo necesite y lo tenemos, se usa automáticamente (ej. una llave)
    Manual,    // Uso manual: lo tenemos pero no se usa hasta que no pulsamos "usar" (ej. una poción o el level up del player)
    None       // No se usa: elementos simplemente intercambiables
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Información básica")]
    [SerializeField] private string itemID;
    [SerializeField] private string itemName;
    [SerializeField] private string description;

    [Header("Representación visual")]
    [SerializeField] private Sprite worldSprite;
    [SerializeField] private Sprite inventorySprite;
    [SerializeField] private GameObject pickupPrefab;

    [Header("Comercio")]
    [SerializeField] private int buyPrice;
    [SerializeField] private int sellPrice;

    [Header("Comportamiento")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private UseType useType;
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStack = 1;

    [Header("Efectos consumibles")]
    [SerializeField] private DropDefinition dropDefinition;
    public DropDefinition DropDefinition => dropDefinition;

    // Getters
    public string ItemID => itemID;
    public string ItemName => itemName;
    public string Description => description;
    public Sprite WorldSprite => worldSprite;
    public Sprite InventorySprite => inventorySprite;
    public GameObject PickupPrefab => pickupPrefab;
    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
    public ItemType Type => itemType;
    public UseType UseType => useType;
    public bool IsStackable => isStackable;
    public int MaxStack => maxStack;
}
