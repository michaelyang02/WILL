using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public void NewGame()
    {
        SceneTransition.Instance("MenuScene").FadeOut("MainGameScene", false);
    }

    public void LoadGame()
    {
        SaveLoadManager.isSaving = false;
        SceneManager.LoadSceneAsync("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void Settings()
    {
        SceneTransition.Instance("MenuScene").FadeOut("SettingsScene", true);
    }

    public void Exit()
    {
        // TODO: Add confirmation window
        Application.Quit();
    }

}
