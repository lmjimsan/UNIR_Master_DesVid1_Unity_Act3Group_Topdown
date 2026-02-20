using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject optionsGroup;

    private void Start()
    {
        // Sincronizar valores iniciales
        musicSlider.value = AudioManager.MusicVolume;
        sfxSlider.value = AudioManager.SfxVolume;

        // Conectar eventos
        musicSlider.onValueChanged.AddListener(AudioManager.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.SetSfxVolume);
    }

    public void OpenSettings()
    {
        optionsGroup.SetActive(true);
    }

    public void CloseSettings()
    {
        optionsGroup.SetActive(false);
    }
}
