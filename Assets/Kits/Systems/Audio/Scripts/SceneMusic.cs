using UnityEngine;

public class SceneMusic : MonoBehaviour
{
	[Header("Music")]
	[SerializeField] private AudioSource musicSource;
	[SerializeField] private AudioClip musicClip;
	[SerializeField] private bool playOnStart = true;
	[SerializeField] private bool loop = true;

	private void Awake()
	{
		if (musicSource == null)
		{
			musicSource = GetComponent<AudioSource>();
		}

		if (musicSource != null)
		{
			if (musicClip != null)
			{
				musicSource.clip = musicClip;
			}

			musicSource.loop = loop;
			ApplyMusicVolume();
		}
	}

	private void OnEnable()
	{
		AudioManager.OnMusicVolumeChanged += HandleMusicVolumeChanged;
		ApplyMusicVolume();
	}

	private void OnDisable()
	{
		AudioManager.OnMusicVolumeChanged -= HandleMusicVolumeChanged;
	}

	private void Start()
	{
		if (playOnStart && musicSource != null)
		{
			if (musicSource.clip == null && musicClip != null)
			{
				musicSource.clip = musicClip;
			}

			ApplyMusicVolume();
			musicSource.Play();
		}
	}

	private void HandleMusicVolumeChanged(float volume)
	{
		if (musicSource != null)
		{
			musicSource.volume = volume;
		}
	}

	private void ApplyMusicVolume()
	{
		if (musicSource != null)
		{
			musicSource.volume = AudioManager.MusicVolume;
		}
	}
}
