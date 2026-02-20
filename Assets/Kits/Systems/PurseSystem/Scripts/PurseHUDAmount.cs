using UnityEngine;
using TMPro;
using System.Globalization;

public class PurseHUDAmount : MonoBehaviour
{
    static readonly CultureInfo SpanishCulture = CultureInfo.GetCultureInfo("es-ES");
    [SerializeField] Purse purseOwner;
    [SerializeField] TMP_Text amountText;

    private void OnEnable()
    {
        if (purseOwner == null)
        {
            purseOwner = GetComponentInParent<Purse>();
        }

        if (purseOwner == null) return;

        purseOwner.OnCoinsChanged.AddListener(OnPurseChange);
    }

    private void OnDisable()
    {
        if (purseOwner == null) return;

        purseOwner.OnCoinsChanged.RemoveListener(OnPurseChange);
    }

    void OnPurseChange(int newAmount)
    {
        amountText.text = newAmount.ToString("N0", SpanishCulture);
    }

    public void SetPurseOwner(Purse newOwner)
    {
        // Desconectar del anterior si existe
        if (purseOwner != null)
        {
            purseOwner.OnCoinsChanged.RemoveListener(OnPurseChange);
        }
        
        // Conectar al nuevo
        purseOwner = newOwner;
        if (purseOwner != null)
        {
            purseOwner.OnCoinsChanged.AddListener(OnPurseChange);
            OnPurseChange(purseOwner.CurrentCoins);
        }
    }
}
