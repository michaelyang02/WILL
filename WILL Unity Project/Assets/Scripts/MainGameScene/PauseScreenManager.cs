using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenManager : MonoBehaviour
{

    public static bool GameIsPaused = false;

    public GameObject fullScreenPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        fullScreenPanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        fullScreenPanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void OpenSettings()
    {
        SceneTransition.Instance("MainGameScene").FadeOut("SettingsScene", true);
        Resume();
    }

    public void ReturnToTitleScreen()
    {
        SceneTransition.Instance("MainGameScene").FadeOut("MenuScene", false);
        Resume();
        // TODO: Add confirmation window
    }

    public void Save()
    {
        Resume();
        SaveLoadManager.isSaving = true;
        SceneManager.LoadSceneAsync("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void Load()
    {
        Resume();
        SaveLoadManager.isSaving = false;
        SceneManager.LoadSceneAsync("SaveLoadScene", LoadSceneMode.Additive);
    }
}
