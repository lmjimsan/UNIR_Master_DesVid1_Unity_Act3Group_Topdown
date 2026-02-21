using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    private Life lifeOwner; 
    [SerializeField] Image imageFill;
    [SerializeField] Image imageHUDLifeBar;

    [Header("Life bar según vida")]
    [SerializeField] Sprite lifebarGreen;
    [SerializeField] Sprite lifebarYellow;
    [SerializeField] Sprite lifebarRed; 

    [Header("Corazones según vida (sólo necesario para HUD)")]
    [SerializeField] Image heartGreen;
    [SerializeField] Image heartYellow;
    [SerializeField] Image heartRed;
    
    [Header("Comportamiento")]
    [SerializeField] bool destroyOnDeath = true;    

    private void OnEnable()
    {
        if (lifeOwner == null)
        {
            lifeOwner = GetComponentInParent<Life>();
        }

        if (lifeOwner == null)
        {
            // Debug.LogError("LifeHUDBar necesita una referencia a Life (lifeOwner).");
            return;
        }

        lifeOwner.OnLifeChanged.AddListener(OnLifeChanged);
        lifeOwner.OnDeath.AddListener(OnDeath);
    }

    private void Start()
    {
        if (lifeOwner != null)
        {
            OnLifeChanged(lifeOwner.CurrentLife);
        }
    }

    private void OnDisable()
    {
        if (lifeOwner == null) return;

        lifeOwner.OnLifeChanged.RemoveListener(OnLifeChanged);
        lifeOwner.OnDeath.RemoveListener(OnDeath);
    }

    void OnLifeChanged(float newLife)
    {
        // Debug.Log($"LifeBar.OnLifeChanged llamado con newLife={newLife}, gameObject active={gameObject.activeSelf}");
        
        float maxLife = lifeOwner != null ? lifeOwner.StartingLife : 1f;
        if (maxLife <= 0f) maxLife = 1f;

        float normalizedLife = Mathf.Clamp01(newLife / maxLife);

        if (normalizedLife > 0.6f)
        {
            imageHUDLifeBar.sprite = lifebarGreen;
            if (heartGreen != null) heartGreen.enabled = true;
            if (heartYellow != null) heartYellow.enabled = false;
            if (heartRed != null) heartRed.enabled = false;
        }
        else if (normalizedLife > 0.3f)
        {
            imageHUDLifeBar.sprite = lifebarYellow;
            if (heartGreen != null) heartGreen.enabled = false;
            if (heartYellow != null) heartYellow.enabled = true;
            if (heartRed != null) heartRed.enabled = false;
        }
        else
        {
            imageHUDLifeBar.sprite = lifebarRed;
            if (heartGreen != null) heartGreen.enabled = false;
            if (heartYellow != null) heartYellow.enabled = false;
            if (heartRed != null) heartRed.enabled = true;
        }
        imageFill.fillAmount = normalizedLife;
    }

    void OnDeath()
    {
        // Debug.Log($"LifeBar.OnDeath llamado, destroyOnDeath={destroyOnDeath}");
        imageFill.fillAmount = 0f;
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        // Si destroyOnDeath es false, simplemente dejamos la barra vacía (no ocultamos nada)
    }

    public void SetLifeOwner(Life newOwner)
    {
        // Desconectar del anterior si existe
        if (lifeOwner != null)
        {
            lifeOwner.OnLifeChanged.RemoveListener(OnLifeChanged);
            lifeOwner.OnDeath.RemoveListener(OnDeath);
        }
        
        // Conectar al nuevo
        lifeOwner = newOwner;
        if (lifeOwner != null)
        {
            lifeOwner.OnLifeChanged.AddListener(OnLifeChanged);
            lifeOwner.OnDeath.AddListener(OnDeath);
            OnLifeChanged(lifeOwner.CurrentLife);
        }
    }
}
