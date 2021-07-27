using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{

    public AudioMixer audioMixer;
    public TMPro.TMP_Dropdown resolutionDropdown;
    public TMPro.TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public Slider writingSpeedSlider;

    private Resolution[] resolutions;


    void Start()
    {
        resolutionDropdown.ClearOptions();

        resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        int currentResolutionIndex = -1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        if (currentResolutionIndex == -1)
        {
            currentResolutionIndex = resolutions.Length - 1;
            SetResolution(currentResolutionIndex);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;

        float volume;
        audioMixer.GetFloat("volume", out volume);
        volumeSlider.value = volume;

        writingSpeedSlider.value = 1f / StoryAnimatedTextManager.WritingTime;
        writingSpeedSlider.maxValue = 60f;
        writingSpeedSlider.minValue = 20f;


        // TODO: save these settings in file and load them next time
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void UnloadSettings()
    {
        SceneTransition.Instance("SettingsScene").FadeOut("", false);
    }

    public void SetWritingSpeed(float writingSpeed)
    {
        StoryAnimatedTextManager.WritingTime = 1f / writingSpeed;
    }
}
