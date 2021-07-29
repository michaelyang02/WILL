using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SaveLoadManager : MonoBehaviour
{
    public static bool isSaving = true;

    public static SaveLoadManager Instance;
    public SaveDatas[] saveDatas;

    void Awake()
    {
        Instance = this;

        // reset
        //SerializationManager.Save("saveData", new SaveDatas[9] { new SaveDatas(), new SaveDatas(), new SaveDatas(), new SaveDatas(), new SaveDatas(), new SaveDatas(), new SaveDatas(), new SaveDatas(), new SaveDatas() });

        saveDatas = SerializationManager.Load<SaveDatas[]>("saveData");
    }

    public void UnloadSavedLoaded()
    {
        SceneManager.UnloadSceneAsync("SaveLoadScene");

        if (SceneManager.GetActiveScene().name == "MainGameScene" && !isSaving)
        {
            MainGameManager.Instance.SquareClick(-1);
            CameraManager.Instance.FocusCamera(Vector2.zero);
            MainGameManager.Instance.GenerateSquares();
            MainGameManager.Instance.GenerateEdges();
        }
        if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            SceneTransition.Instance("MenuScene").FadeOut("MainGameScene", false);
        }
    }

    public void Unload()
    {
        SceneManager.UnloadSceneAsync("SaveLoadScene");
    }
}
