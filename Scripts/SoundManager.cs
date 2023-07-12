using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Button muteButton;
    public Button unmuteButton;

    private float previousVolume;
    private bool isFirstStartup = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.25f);
            Load();
            isFirstStartup = true; // Set isFirstStartup to true only during the first startup
        }
        else
        {
            Load();
            isFirstStartup = false; // Set isFirstStartup to false if the value already exists in PlayerPrefs
        }

        volumeSlider.onValueChanged.AddListener(VolumeSliderChanged);
        muteButton.onClick.AddListener(Mute);
        unmuteButton.onClick.AddListener(Unmute);

        UpdateMuteUnmuteButtons();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
        UpdateMuteUnmuteButtons();
    }

    public void VolumeSliderChanged(float value)
    {
        UpdateMuteUnmuteButtons();
    }

    public void Mute()
    {
        previousVolume = volumeSlider.value;
        volumeSlider.value = 0;
        AudioListener.volume = 0;
        Save();
        UpdateMuteUnmuteButtons();
    }

    public void Unmute()
    {
        volumeSlider.value = previousVolume;
        AudioListener.volume = previousVolume;
        Save();
        UpdateMuteUnmuteButtons();
    }

    private void Load()
    {
        float savedVolume = PlayerPrefs.GetFloat("musicVolume");
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

    private void UpdateMuteUnmuteButtons()
    {
        bool isMuted = volumeSlider.value <= 0;
        muteButton.gameObject.SetActive(!isMuted);
        unmuteButton.gameObject.SetActive(isMuted);

        // Set slider to 0.25 on first startup
        if (isFirstStartup)
        {
            volumeSlider.value = 0.25f;
            AudioListener.volume = 0.25f;
            Save();
            isFirstStartup = false; // Reset isFirstStartup after setting the default value
        }
    }
}
