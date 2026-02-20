using UnityEngine;

/// <summary>
/// Gestor global de audio que controla volúmenes de música y SFX.
/// Funciona como Singleton con DontDestroyOnLoad.
/// Se debe instanciar una vez en el Main Menu o en una escena inicial.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null)
        {
            return;
        }

        AudioManager existing = FindFirstObjectByType<AudioManager>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        GameObject audioManagerGO = new GameObject("AudioManager");
        audioManagerGO.AddComponent<AudioManager>();
    }

    [Header("Volume Settings")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.3f;
    // El volumen se multiplica x3 internamente, por eso el rango es 0-1 pero suena como si fuera 0-3
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.5f;

    // Propiedades estáticas para acceso global
    public static float MusicVolume => Instance != null ? Instance.musicVolume : 0.3f;
    // SFX amplificado x10 para prueba (Unity lo clampea automáticamente a 1.0)
    public static float SfxVolume => Instance != null ? Instance.sfxVolume * 10f : 0.5f * 10f;

    // Eventos que se disparan cuando cambian los volúmenes
    public static event System.Action<float> OnMusicVolumeChanged;
    public static event System.Action<float> OnSfxVolumeChanged;

    private void Awake()
    {
        // Implementar patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargar volúmenes guardados (opcional)
        LoadAudioSettings();
    }

    private void LoadAudioSettings()
    {
        // Si tienes PlayerPrefs guardados, cárgatlos aquí
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        }
        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.7f);
        }
    }

    /// <summary>
    /// Establece el volumen de música y dispara el evento
    /// </summary>
    public static void SetMusicVolume(float value)
    {
        if (Instance == null) return;

        Instance.musicVolume = Mathf.Clamp01(value);
        OnMusicVolumeChanged?.Invoke(Instance.musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", Instance.musicVolume);
    }

    /// <summary>
    /// Establece el volumen de SFX y dispara el evento
    /// </summary>
    public static void SetSfxVolume(float value)
    {
        if (Instance == null) return;

        Instance.sfxVolume = Mathf.Clamp01(value);
        OnSfxVolumeChanged?.Invoke(Instance.sfxVolume);
        PlayerPrefs.SetFloat("SfxVolume", Instance.sfxVolume);
    }
}
