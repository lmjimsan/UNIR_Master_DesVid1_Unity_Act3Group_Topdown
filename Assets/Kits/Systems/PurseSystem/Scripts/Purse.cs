using UnityEngine;
using UnityEngine.Events;

public class Purse : MonoBehaviour
{
    [SerializeField] int startingCoins = 10;

    [Header("Debug")]
    [SerializeField] int debugAmountToIncrease = 100;
    [SerializeField] bool debugReceiveCoins = false;

    [SerializeField] int currentCoins;

    [SerializeField] public UnityEvent<int> OnCoinsChanged;

    public int CurrentCoins => currentCoins;

    private void Awake()
    {
        currentCoins = startingCoins;
    }
     
    private void OnValidate()
    {
        if (debugReceiveCoins)
        {
            debugReceiveCoins = false;
            OnGetCoins(debugAmountToIncrease);
        }
    }

    // Si no hay suficiente dinero, no se hace nada y se devuelve false.
    public bool OnGiveCoins(int amount)
    {
        if (currentCoins > amount)
        {
            currentCoins -= amount;
            OnCoinsChanged.Invoke(currentCoins);
            return true;
        }  
        return false;
    }

    public void OnGetCoins(int amount)
    {
        currentCoins += amount;
        OnCoinsChanged.Invoke(currentCoins);
    }
}
