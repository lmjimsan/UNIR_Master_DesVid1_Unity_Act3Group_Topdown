using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class PlayerSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Animación de subida de nivel")]
    [SerializeField] private GameObject levelUpAnimationObject;

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
        // Detectar si estamos sobre el inventario y consumir con botón izquierdo
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            bool storeOpen = storeUI != null && storeUI.gameObject.activeInHierarchy;
            if (!storeOpen && eventData.button == PointerEventData.InputButton.Left && currentItem != null)
            {
                // Debug.Log($"Click izquierdo sobre slot {SlotIndex} con item: {currentItem.ItemName}");
                if (currentItem.UseType == UseType.Manual)
                {
                    var player = FindFirstObjectByType<PlayerCharacter>();
                    var dropDef = currentItem.DropDefinition;
                    Debug.Log($"[DEBUG] currentItem={currentItem?.ItemName}, dropDef null={dropDef==null}, player null={player==null}, player.Life null={player?.Life==null}");
                    if (dropDef != null && player != null && player.Life != null)
                    {
                        Debug.Log($"[DEBUG] Entrando en bloque de aplicación de efectos DropDefinition");
                        Debug.Log($"[DEBUG] Valores: healthRecovery={dropDef.healthRecovery}, powerUpDamage={dropDef.powerUpDamage}, powerUpShield={dropDef.powerUpShield}, powerUpDuration={dropDef.powerUpDuration}");
                        if (dropDef.healthRecovery > 0)
                        {
                            player.Life.RecoverHealth(dropDef.healthRecovery);
                            Debug.Log($"[DEBUG] Vida antes: {player.Life.CurrentLife - dropDef.healthRecovery}, después: {player.Life.CurrentLife}, incremento: {dropDef.healthRecovery}");
                        }
                        if (dropDef.powerUpDamage > 0)
                        {
                            float prevDamage = player.damage;
                            player.damage += dropDef.powerUpDamage;
                            Debug.Log($"[DEBUG] Daño antes: {prevDamage}, después: {player.damage}, incremento: {dropDef.powerUpDamage}");
                        }
                        if (dropDef.powerUpShield > 0)
                        {
                            float prevShield = player.shield;
                            player.shield += dropDef.powerUpShield;
                            Debug.Log($"[DEBUG] Escudo antes: {prevShield}, después: {player.shield}, incremento: {dropDef.powerUpShield}");
                        }
                        if (dropDef.powerUpDuration > 0 && (dropDef.powerUpDamage > 0 || dropDef.powerUpShield > 0))
                        {
                            player.StartCoroutine(RevertPowerUp(player, dropDef.powerUpDamage, dropDef.powerUpShield, dropDef.powerUpDuration));
                        }

                        // Cambiar skin según el nivel de daño
                        var animator = player.GetComponent<Animator>();
                        int prevLevel = (player.damage - (dropDef.powerUpDamage > 0 ? dropDef.powerUpDamage : 0) <= 0.5f) ? 1 : (player.damage - (dropDef.powerUpDamage > 0 ? dropDef.powerUpDamage : 0) < 3.5f ? 2 : 3);
                        int newLevel = (player.damage <= 0.5f) ? 1 : (player.damage < 3.5f ? 2 : 3);
                        if (animator != null)
                        {
                            RuntimeAnimatorController level1 = Resources.Load<RuntimeAnimatorController>("PlayerL1AnimationController");
                            RuntimeAnimatorController level2 = Resources.Load<RuntimeAnimatorController>("PlayerL2AnimatorOverrideController");
                            RuntimeAnimatorController level3 = Resources.Load<RuntimeAnimatorController>("PlayerL3AnimationOverrideController");
                            if (player.damage <= 0.5f && level1 != null)
                                animator.runtimeAnimatorController = level1;
                            else if (player.damage > 0.5f && player.damage < 3.5f && level2 != null)
                                animator.runtimeAnimatorController = level2;
                            else if (player.damage >= 3.5f && level3 != null)
                                animator.runtimeAnimatorController = level3;
                        }

                        // Reproducir sonido si cambia de nivel
                        if (playerInventoryUI != null && newLevel != prevLevel)
                        {
                            playerInventoryUI.PlayLevelChangeSound(prevLevel, newLevel);
                            PlayLevelUpAnimation();
                        }
                    }
                    // Eliminar el ítem del inventario
                    playerInventoryUI.GetInventory().TryRemoveItem(currentItem);
                    playerInventoryUI.RefreshInventoryUI();
                }
            }
            else if (storeUI != null && eventData.button == PointerEventData.InputButton.Left)
            {
                storeUI.TransferItemFromPlayerToFirstEmptySlot(this);
            }
        }
    }

    /// Activa el objeto de animación de subida de nivel durante 0.1 segundos.
    public void PlayLevelUpAnimation()
    {
        if (levelUpAnimationObject != null)
        {
            levelUpAnimationObject.SetActive(true);
            StartCoroutine(DisableLevelUpAnimationAfterDelay());
        }
    }

    private IEnumerator DisableLevelUpAnimationAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        if (levelUpAnimationObject != null)
            levelUpAnimationObject.SetActive(false);
    }

    // Corrutina para revertir power up
    private IEnumerator RevertPowerUp(PlayerCharacter player, float damage, float shield, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (damage > 0) player.damage -= damage;
        if (shield > 0) player.shield -= shield;
        // Debug.Log($"PowerUp revertido: -{damage} damage, -{shield} shield");
        // Cambiar skin según el nivel de daño tras revertir
        var animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            RuntimeAnimatorController level1 = Resources.Load<RuntimeAnimatorController>("PlayerL1AnimationController");
            RuntimeAnimatorController level2 = Resources.Load<RuntimeAnimatorController>("PlayerL2AnimatorOverrideController");
            RuntimeAnimatorController level3 = Resources.Load<RuntimeAnimatorController>("PlayerL3AnimationOverrideController");
            if (player.damage <= 0.5f && level1 != null)
                animator.runtimeAnimatorController = level1;
            else if (player.damage > 0.5f && player.damage < 3.5f && level2 != null)
                animator.runtimeAnimatorController = level2;
            else if (player.damage >= 3.5f && level3 != null)
                animator.runtimeAnimatorController = level3;
        }
    }
}