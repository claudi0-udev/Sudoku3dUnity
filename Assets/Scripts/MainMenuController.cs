using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static bool isLoadingBoard = false;
    [SerializeField] private GameObject continueGameButton;
    [SerializeField] private Image blackScreenImage;

    void Awake()
    {
        if (blackScreenImage != null)
            blackScreenImage.color = new Color(0, 0, 0, 1);
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (ExistSaveFile())
            {
                continueGameButton.SetActive(true);
            }
            else
            {
                continueGameButton.SetActive(false);
            }


            FadeOutImage(2);                  

        }

        Time.timeScale = 1;
    }

    public bool ExistSaveFile()
    {
        string folder_path = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);
        string file_game_time_name = "game_time_data.json";
        string file_game_data_name = "game_data.json";
        string file_game_time_path = folder_path + "/" + file_game_time_name;
        string file_game_data_path = folder_path + "/" + file_game_data_name;

        if(File.Exists(file_game_data_path) && File.Exists(file_game_time_path))
        {
            Debug.Log("Ambos archivos existen!!!");
            return true;
        }
        else
        {
            Debug.Log("No existen los archivos o hay error!!!");
            return false;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame(string sceneName)
    {
        isLoadingBoard = false;
        StartCoroutine(LoadSceneIE(2, sceneName));

        if (!GameObject.FindObjectOfType<MainMenuController>())
            DontDestroyOnLoad(this);
    }

    public void ContinueGame(string sceneName)
    {
        isLoadingBoard = true;
        StartCoroutine(LoadSceneIE(2, sceneName));

        if(!GameObject.FindObjectOfType<MainMenuController>())
            DontDestroyOnLoad(this);
    }

    IEnumerator LoadSceneIE(float time, string sceneName)
    {
        yield return new WaitForSeconds(time);
        LoadScene(sceneName);
        yield break;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitGameInSeconds(float seconds)
    {
        Debug.Log($"Quit in {seconds} seconds");
        Invoke("ExitGame", seconds);
    }

    public void FadeOutImage(float time)
    {
        if (blackScreenImage != null)
            blackScreenImage.CrossFadeAlpha(0, time, true);
    }

    public void FadeInImage(float time)
    {
        if (blackScreenImage != null)
            blackScreenImage.CrossFadeAlpha(1, time, true);
    }
}
