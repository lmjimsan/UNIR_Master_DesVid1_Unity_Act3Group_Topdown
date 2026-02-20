using UnityEngine;
using UnityEngine.Events;

public class Life : MonoBehaviour
{
    [SerializeField] float startingLife = 1f;

    public float StartingLife => startingLife;
    public float CurrentLife => currentLife;

    [Header("Debug")]
    [SerializeField] float debugHitDamage = 0.1f;
    [SerializeField] bool debugReceiveHit = false;

    [SerializeField] float currentLife;

    [SerializeField] public UnityEvent<float> OnLifeChanged;
    [SerializeField] public UnityEvent OnDeath;

    private void Awake()
    {
        currentLife = startingLife;
    }
     
    private void OnValidate()
    {
        if (debugReceiveHit)
        {
            debugReceiveHit = false;
            if (currentLife <= 0f) currentLife = startingLife;
            OnHitReceived(debugHitDamage);
        }
    }

    public void OnHitReceived(float damage)
    {
        if (currentLife > 0f)
        {
            currentLife -= damage;
            OnLifeChanged.Invoke(currentLife);
            if (currentLife <= 0f)
            {
                currentLife = 0;
                OnDeath.Invoke();
            }
        }
    }

    public void RecoverHealth(float amountHealth)
    {
        if (currentLife > 0f)
        {
            currentLife += amountHealth;
            currentLife = Mathf.Clamp01(currentLife);
            OnLifeChanged.Invoke(currentLife);
        }
    }

    public void Respawn()
    {
        currentLife = startingLife;
        OnLifeChanged.Invoke(currentLife);
        Debug.Log($"Life.Respawn() - currentLife restaurada a {currentLife}");
    }
}
