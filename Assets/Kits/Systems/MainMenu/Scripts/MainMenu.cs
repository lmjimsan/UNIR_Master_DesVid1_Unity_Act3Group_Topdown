using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Imagen/titulo que queremos mover dentro del menu
    [SerializeField] private RectTransform kamaelTitle;
    // Activa o desactiva el movimiento desde el Inspector
    [SerializeField] private bool moveImage = true;
    // Margen total de movimiento (ancho y alto) tomando como centro la posicion inicial
    [SerializeField] private Vector2 movementDelta = new Vector2(500f, 200f);
    // Velocidad del rebote en pixeles por segundo
    [SerializeField] private float moveSpeed = 20f;

    [Header("UI")]
    // Raycaster del Canvas para detectar imagenes con el raton
    [SerializeField] private GraphicRaycaster uiRaycaster;
    // EventSystem de la escena (necesario para los raycasts de UI)
    [SerializeField] private EventSystem eventSystem;
    // Ventana de creditos que se activa al pulsar el boton
    [SerializeField] private GameObject creditsWin;
    // Grupo de opciones que se activa al pulsar el boton
    [SerializeField] private GameObject optionsGroup;
    // Nombre de la escena a cargar al pulsar Play
    [SerializeField] private string playSceneName = "Home";

    [Header("Botones (Image)")]
    // Imagenes que actuan como botones
    [SerializeField] private Image playButton;
    [SerializeField] private Image creditsButton;
    [SerializeField] private Image optionsButton;
    [SerializeField] private Image quitButton;
    [SerializeField] private Image quitWinCreditsButton;
    [SerializeField] private Image quitWinOptionsButton;
    // Sprites normal/pressed para cada boton
    [SerializeField] private Sprite playNormal;
    [SerializeField] private Sprite playPressed;
    [SerializeField] private Sprite creditsNormal;
    [SerializeField] private Sprite creditsPressed;
    [SerializeField] private Sprite optionsNormal;
    [SerializeField] private Sprite optionsPressed;
    [SerializeField] private Sprite quitNormal;
    [SerializeField] private Sprite quitPressed;
    [SerializeField] private Sprite quitWinNormal;
    [SerializeField] private Sprite quitWinPressed;

    [Header("Audio")]
    // Fuente de musica de fondo
    [SerializeField] private AudioSource musicSource;
    // Clip de musica de fondo
    [SerializeField] private AudioClip musicClip;
    // Fuente de efectos de sonido
    [SerializeField] private AudioSource sfxSource;
    // Clip del sonido de click
    [SerializeField] private AudioClip clickSfx;

    // Buffer reutilizado para resultados de raycast de UI
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();
    // Velocidad actual del titulo en la UI
    private Vector2 velocity;
    // Posicion inicial que usamos como centro del movimiento
    private Vector2 originPosition;
    // Imagen que se esta presionando con el raton
    private Image pressedImage;

    // Se ejecuta al iniciar la escena
    void Start()
    {
        // Borrar PlayerPrefs al entrar en el menú principal
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Asegurar que existe el AudioManager
        if (AudioManager.Instance == null)
        {
            GameObject audioManagerGO = new GameObject("AudioManager");
            audioManagerGO.AddComponent<AudioManager>();
        }

        // Credits apagado al iniciar la escena
        if (creditsWin != null)
        {
            creditsWin.SetActive(false);
        }

        // Options apagado al iniciar la escena
        if (optionsGroup != null)
        {
            optionsGroup.SetActive(false);
        }

        // Guardamos la posicion inicial para usarla como centro del movimiento
        if (kamaelTitle != null)
        {
            originPosition = kamaelTitle.anchoredPosition;
            velocity = GetInitialVelocity();
        }

        // Arranca la musica de fondo en bucle
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            ApplyAudioVolumes();
            musicSource.Play();
        }
    }

    private void OnEnable()
    {
        AudioManager.OnMusicVolumeChanged += HandleMusicVolumeChanged;
        AudioManager.OnSfxVolumeChanged += HandleSfxVolumeChanged;
        ApplyAudioVolumes();
    }

    // Se ejecuta cada frame
    void Update()
    {
        // Gestiona el click del raton sobre las imagenes
        HandleMouseInput();

        // Si no queremos mover o no hay imagen, salimos
        if (!moveImage || kamaelTitle == null) return;

        // Calculamos los limites del rectangulo de movimiento
        Vector2 halfDelta = movementDelta * 0.5f;
        Vector2 minBounds = originPosition - halfDelta;
        Vector2 maxBounds = originPosition + halfDelta;
        Vector2 current = kamaelTitle.anchoredPosition;

        // Asegura que la velocidad mantiene la magnitud indicada
        if (velocity.sqrMagnitude < 0.001f)
        {
            velocity = GetInitialVelocity();
        }
        else
        {
            velocity = velocity.normalized * moveSpeed;
        }

        // Avanza la posicion con rebote tipo pelota
        Vector2 next = current + (velocity * Time.deltaTime);

        // Si toca el borde en X, rebota
        if (next.x <= minBounds.x || next.x >= maxBounds.x)
        {
            velocity.x = -velocity.x;
            next.x = Mathf.Clamp(next.x, minBounds.x, maxBounds.x);
        }

        // Si toca el borde en Y, rebota
        if (next.y <= minBounds.y || next.y >= maxBounds.y)
        {
            velocity.y = -velocity.y;
            next.y = Mathf.Clamp(next.y, minBounds.y, maxBounds.y);
        }

        // Aplicamos la nueva posicion
        kamaelTitle.anchoredPosition = next;
    }

    // Se ejecuta cuando se desactiva el objeto o se cambia de escena
    void OnDisable()
    {
        AudioManager.OnMusicVolumeChanged -= HandleMusicVolumeChanged;
        AudioManager.OnSfxVolumeChanged -= HandleSfxVolumeChanged;

        // Para la musica al salir de la escena
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // Detecta pulsaciones del raton sobre las imagenes de UI
    private void HandleMouseInput()
    {
        if (uiRaycaster == null || eventSystem == null) return;

        // Al pulsar, guardamos la imagen y cambiamos su sprite
        if (Input.GetMouseButtonDown(0))
        {
            pressedImage = GetImageUnderMouse();
            if (pressedImage != null)
            {
                SetPressedSprite(pressedImage, true);
                PlayClick();
            }
        }

        // Al soltar, restauramos el sprite y ejecutamos la accion si procede
        if (Input.GetMouseButtonUp(0))
        {
            Image releasedImage = GetImageUnderMouse();
            if (pressedImage != null)
            {
                SetPressedSprite(pressedImage, false);
            }

            if (pressedImage != null && releasedImage == pressedImage)
            {
                HandleButtonAction(pressedImage);
            }

            pressedImage = null;
        }
    }

    // Devuelve la imagen de boton que hay bajo el raton, si la hay
    private Image GetImageUnderMouse()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        raycastResults.Clear();
        uiRaycaster.Raycast(pointerData, raycastResults);

        // Recorremos resultados y devolvemos el primero que sea uno de nuestros botones
        for (int i = 0; i < raycastResults.Count; i++)
        {
            Image image = raycastResults[i].gameObject.GetComponent<Image>();
            if (image != null && (image == playButton || image == creditsButton || image == optionsButton || image == quitButton || image == quitWinCreditsButton || image == quitWinOptionsButton))
            {
                return image;
            }
        }

        return null;
    }

    // Cambia el sprite del boton al estado normal/pressed
    private void SetPressedSprite(Image image, bool isPressed)
    {
        if (image == playButton && playPressed != null && playNormal != null)
        {
            playButton.sprite = isPressed ? playPressed : playNormal;
        }
        else if (image == creditsButton && creditsPressed != null && creditsNormal != null)
        {
            creditsButton.sprite = isPressed ? creditsPressed : creditsNormal;
        }
        else if (image == optionsButton && optionsPressed != null && optionsNormal != null)
        {
            optionsButton.sprite = isPressed ? optionsPressed : optionsNormal;
        }
        else if (image == quitButton && quitPressed != null && quitNormal != null)
        {
            quitButton.sprite = isPressed ? quitPressed : quitNormal;
        }
        else if (image == quitWinCreditsButton && quitWinPressed != null && quitWinNormal != null)
        {
            quitWinCreditsButton.sprite = isPressed ? quitWinPressed : quitWinNormal;
        }
        else if (image == quitWinOptionsButton && quitWinPressed != null && quitWinNormal != null)
        {
            quitWinOptionsButton.sprite = isPressed ? quitWinPressed : quitWinNormal;
        }
    }

    // Ejecuta la accion asociada a cada boton
    private void HandleButtonAction(Image image)
    {
        if (image == creditsButton)
        {
            ToggleCredits();
        }
        else if (image == optionsButton)
        {
            ToggleOptions();
        }
        else if (image == playButton)
        {
            PlayGame();
        }
        else if (image == quitButton)
        {
            QuitGame();
        }
        else if (image == quitWinCreditsButton)
        {
            HideCredits();
        }
        else if (image == quitWinOptionsButton)
        {
            HideOptions();
        }
    }

    // Alterna la ventana de créditos
    private void ToggleCredits()
    {
        HideOptions();
        if (creditsWin != null)
        {
            creditsWin.SetActive(!creditsWin.activeSelf);
        }
    }

    // Alterna la ventana de opciones
    private void ToggleOptions()
    {
        HideCredits();
        if (optionsGroup != null)
        {
            optionsGroup.SetActive(!optionsGroup.activeSelf);
        }
    }

    // Muestra la ventana de creditos
    private void ShowCredits()
    {
        HideOptions();
        if (creditsWin != null)
        {
            creditsWin.SetActive(true);
        }
    }

    // Muestra la ventana de opciones
    private void ShowOptions()
    {
        HideCredits();
        if (optionsGroup != null)
        {
            optionsGroup.SetActive(true);
        }
    }

    // Carga la escena del juego
    private void PlayGame()
    {
        if (!string.IsNullOrWhiteSpace(playSceneName))
        {
            SceneManager.LoadScene(playSceneName);
        }
    }

    // Oculta la ventana de creditos
    private void HideCredits()
    {
        if (creditsWin != null)
        {
            creditsWin.SetActive(false);
        }
    }

    // Oculta la ventana de opciones
    private void HideOptions()
    {
        if (optionsGroup != null)
        {
            optionsGroup.SetActive(false);
        }
    }

    // Sale del juego (en el editor no se cierra)
    private void QuitGame()
    {
        Application.Quit();
    }

    // Reproduce el sonido de click
    private void PlayClick()
    {
        if (sfxSource != null && clickSfx != null)
        {
            sfxSource.volume = AudioManager.SfxVolume;
            sfxSource.PlayOneShot(clickSfx);
        }
    }

    private void HandleMusicVolumeChanged(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    private void HandleSfxVolumeChanged(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    private void ApplyAudioVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = AudioManager.MusicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = AudioManager.SfxVolume;
        }
    }

    // Genera una direccion inicial aleatoria con la velocidad indicada
    private Vector2 GetInitialVelocity()
    {
        return Random.insideUnitCircle.normalized * moveSpeed;
    }
}
