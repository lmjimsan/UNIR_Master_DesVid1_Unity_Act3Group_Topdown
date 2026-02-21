using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Script que muestra un texto letra a letra con sonido opcional.
/// Se debe colocar en un GameObject que contenga un TextMeshProUGUI.
/// </summary>
public class DialogueTypewriter : MonoBehaviour
{
    [Header("Auto-cierre")]
    [Tooltip("Segundos a esperar tras terminar de escribir antes de cerrar el diálogo (0 = no cerrar automáticamente)")]
    [SerializeField] public float autoCloseDelay = 0f;
    private MonoBehaviour autoCloseTarget;
    public event System.Action OnTypingComplete;
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private float delayBetweenLetters = 0.05f;
    [SerializeField] private float delayEndMessage = 2f;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private float soundPitch = 1f;
    
    private Coroutine typingCoroutine;
    private AudioSource audioSource;

    private void Awake()
    {
        // Intentar obtener el TextMeshProUGUI del mismo GameObject o de los hijos
        if (textDisplay == null)
        {
            textDisplay = GetComponent<TextMeshProUGUI>();
        }
        if (textDisplay == null)
        {
            textDisplay = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Buscar o crear un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Inicia la visualización del texto letra a letra.
    /// Si ya hay una corrutina en curso, la cancela primero.
    /// </summary>
    public void StartTyping(string fullText)
    {
        // Detener la corrutina anterior si existe
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    /// <summary>
    /// Detiene el efecto de tipeo y muestra el texto completo.
    /// </summary>
    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (textDisplay != null)
        {
            textDisplay.maxVisibleCharacters = int.MaxValue;
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        textDisplay.text = fullText;
        textDisplay.maxVisibleCharacters = 0;

        for (int i = 0; i < fullText.Length; i++)
        {
            textDisplay.maxVisibleCharacters = i + 1;

            // Reproducir sonido si está asignado
            if (typingSound != null && audioSource != null)
            {
                audioSource.pitch = soundPitch;
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(delayBetweenLetters);
        }

        yield return new WaitForSeconds(delayEndMessage);

        typingCoroutine = null;
        if (autoCloseDelay > 0)
        {
            if (autoCloseTarget != null)
                autoCloseTarget.StartCoroutine(AutoCloseCoroutine());
            else
                StartCoroutine(AutoCloseCoroutine());
        }
        else
        {
            OnTypingComplete?.Invoke();
        }
    }

    private IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        OnTypingComplete?.Invoke();
    }

    /// <summary>
    /// Permite que un script externo (ej: ShopKeeper) asigne el MonoBehaviour para correr la corrutina de autocierre.
    /// </summary>
    public void SetAutoCloseTarget(MonoBehaviour target)
    {
        autoCloseTarget = target;
    }
}