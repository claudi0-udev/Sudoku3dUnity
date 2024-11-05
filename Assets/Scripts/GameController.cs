using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject cameraToDesactivate;

    public LayerMask touchLayerMask;

    public GameObject onMovingObject;

    public Transform canvasParent;



    GameObject objectOnMoving = null; //Sirve para mover el numero que se intenta asignar a la casilla

    int valueOnMoving = 0;

    public GenerateBoard generateBoard;

    private Casilla casillaToDelete = null;

    [SerializeField] private bool isDeletingCasilla = false;

    [SerializeField]int size = 9;

    [SerializeField] Font homemadeApple;

    [SerializeField] Font arial;

    [SerializeField] private float gameTime, previousGameTime;

    [SerializeField] private Text gameTimeText;

    [SerializeField] private ParticleSystem eraseEffectParticleSystem;
    [SerializeField] private ParticleSystem saveEffectParticleSystem;

    [SerializeField] private Image blackScreenImage;
    [SerializeField] private GameObject gameCompletedPanel;
    [SerializeField] private Text gameCompletedMessageText;


    // Start is called before the first frame update
    void Start()
    {
        size = generateBoard.size;
        if (MainMenuController.isLoadingBoard)
        {
            LoadGame();
            MainMenuController.isLoadingBoard = false;
        }
        Time.timeScale = 1;
        PauseScene.isGamePaused = false;

        FadeOutImage(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseScene.isGamePaused) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(touch.position, Vector2.right, .1f, touchLayerMask);
                if (hit)
                {
                    if (hit.transform.CompareTag("Number"))
                    {
                        Debug.Log(hit.transform.GetChild(0).GetComponent<Text>().text);
                        string valueToCast = hit.transform.GetChild(0).GetComponent<Text>().text;
                        valueOnMoving = int.Parse(valueToCast);
                        objectOnMoving = Instantiate(onMovingObject, canvasParent);
                        objectOnMoving.transform.position = hit.transform.position;
                        objectOnMoving.transform.GetChild(0).GetComponent<Text>().text = valueToCast;
                    }
                        
                }
                else
                {
                    RaycastHit hit3D;
                    if(Physics.Raycast(Camera.main.ScreenToWorldPoint((Vector3)touch.position), Vector3.forward, out hit3D, 20.0f, touchLayerMask))
                    {
                        if (hit3D.transform.CompareTag("Number"))
                        {
                            Debug.Log(hit3D.transform.gameObject.name + " ON3D");
                            casillaToDelete = hit3D.transform.GetComponent<Casilla>();
                            isDeletingCasilla = true;
                        }
                    }
                }
            }
            if(touch.phase == TouchPhase.Moved)
            {
                Debug.Log("Se esta moviendo");
                if (objectOnMoving != null)
                {
                    objectOnMoving.transform.position = touch.position;                    
                }
            }

            if(touch.phase == TouchPhase.Ended)
            {
                Debug.Log("touch phase ended");
                if(objectOnMoving != null)
                {
                    RaycastHit hit;
                    if(Physics.Raycast(Camera.main.ScreenToWorldPoint((Vector3)touch.position), Vector3.forward, out hit, 20.0f, touchLayerMask))
                    {
                        Debug.Log(hit.transform.gameObject.name);

                        hit.transform.gameObject.SendMessage("SetValue", valueOnMoving);
                        CheckBoard();
                    }
                    //
                    Destroy(objectOnMoving);                   
                    
                }

                //Metodos 3D hit3D
                if (isDeletingCasilla)
                {
                    if (casillaToDelete != null)//en caso que el raycast haya dado con una casilla en 3d a borrar
                    {
                        if (casillaToDelete.isWritable)
                        {
                            //Efecto de borrado en UI
                            eraseEffectParticleSystem.transform.position = casillaToDelete.transform.position + Vector3.forward * -0.5f;
                            eraseEffectParticleSystem.Play();
                            //Borrado
                            casillaToDelete.SetValue(0);
                            Debug.Log(casillaToDelete.transform.gameObject.name + " Deleted");
                            casillaToDelete = null;
                            isDeletingCasilla = false;
                            CheckBoard();                            
                        }
                    }
                }
            }
        }

        GameTimer();
        
    }

    void CheckBoard()
    {
        ResetErrors();
        CheckRows();
        CheckColumns();
        CheckBlocks();
        ResetColor();

        if (IsGameCompleted())
        {
            Debug.Log("Juego completado con exito!");
            //blackScreenImage.color = new Color(0, 0, 0, .5f);
            blackScreenImage.CrossFadeAlpha(.5f, 1f, true);
            blackScreenImage.raycastTarget = true;
            gameCompletedPanel.SetActive(true);
            PauseScene.isGamePaused = true;
            Time.timeScale = 0;
            gameCompletedMessageText.text = $"Game completed in {(int)gameTime} seconds";
            
        }
        else
        {
            Debug.Log("Juego aun sin completar");
        }
    }

    void CheckRows()
    {
        for(int x = 0; x < size; x++)
        {
            List<Casilla> r = new List<Casilla>();
            for(int y = 0; y < size; y++)
            {
                if(GenerateBoard.row[x, y].value != 0)
                    r.Add(GenerateBoard.row[x, y]);
            }            

            r = r.OrderBy(n => n.GetComponent<Casilla>().value).ToList();

            for(int i = 1; i < r.Count; i++)
            {

                if (r[i].value == r[i - 1].value)
                {
                    r[i].error = true;
                    r[i - 1].error = true;
                }
            }
            
        }
    }

    void CheckColumns()
    {
        for (int x = 0; x < size; x++)
        {
            List<Casilla> r = new List<Casilla>();
            for (int y = 0; y < size; y++)
            {
                if (GenerateBoard.column[x, y].value != 0)
                    r.Add(GenerateBoard.column[x, y]);
            }

            r = r.OrderBy(n => n.GetComponent<Casilla>().value).ToList();

            for (int i = 1; i < r.Count; i++)
            {

                if (r[i].value == r[i - 1].value)
                {
                    r[i].error = true;
                    r[i - 1].error = true;
                }
            }

        }
    }

    void CheckBlocks()
    {
        for (int x = 0; x < size; x++)
        {
            List<Casilla> r = new List<Casilla>();
            for (int y = 0; y < size; y++)
            {
                if (GenerateBoard.block[x, y].value != 0)
                    r.Add(GenerateBoard.block[x, y]);
            }

            r = r.OrderBy(n => n.GetComponent<Casilla>().value).ToList();

            for (int i = 1; i < r.Count; i++)
            {

                if (r[i].value == r[i - 1].value)
                {
                    r[i].error = true;
                    r[i - 1].error = true;
                }
            }

        }
    }

    public bool IsGameCompleted()
    {
        foreach(Casilla casilla in GenerateBoard.row)
        {
            if(casilla.value == 0)
            {
                return false;
            }

            if (casilla.error)
            {
                return false;
            }
        }

        return true;
    }

    void ResetColor()
    {
        foreach (Casilla ca in GenerateBoard.row)//se puede usar row, block, column. El orden no altera el producto
        {
            if (!ca.error)
                ca.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", ca.color);
            else
                ca.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", generateBoard.errorColor);
        }
    }

    void ResetErrors()
    {
        foreach (Casilla ca in GenerateBoard.row)//se puede usar row, block, column. El orden no altera el producto
        {
            ca.error = false;
        }
    }

    void GameTimer()
    {
        gameTime += Time.deltaTime;
        if ((int)gameTime == (int)(previousGameTime + .7f))
            return;

        int hours = (int)((gameTime / 60) / 60);
        int minutes = (int)(gameTime / 60) % 60;
        int seconds = (int)(gameTime % 60);

        //gameTimeText.text = TimeFormatter(hours) + " : " + TimeFormatter(minutes) + " : " + TimeFormatter(seconds);
        gameTimeText.text = $"{hours.ToString("00")} : {minutes.ToString("00")} : {seconds.ToString("00")}";

        Debug.Log("Ejecutando TIMER");
        previousGameTime = gameTime;
    }

    string TimeFormatter(int n)
    {
        string val;
        if (n.ToString().Trim().Length <= 1)
            val = "0" + n.ToString().Trim();
        else
            val = n.ToString().Trim();
        return val;
    }


    public void DesactivateCamera()
    {
        cameraToDesactivate.SetActive(!cameraToDesactivate.active);
    }

    public void SaveGame()
    {
        if (PauseScene.isGamePaused) return;

        //Visual
        saveEffectParticleSystem.Play();
        //--------------//

        string folder_path = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);
        string file_name = "game_data.json";
        string file_path = folder_path + "/" + file_name;
        //-----------------------------------------//

        ValuesToSaveCasilla[,] c = new ValuesToSaveCasilla[9, 9];
        for (int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                c[x, y] = new ValuesToSaveCasilla();
                c[x, y].value = GenerateBoard.row[x, y].value;
                c[x, y].isWritable = GenerateBoard.row[x, y].isWritable;
            }
        }
        //-----------------------------------------//
        
        try
        {
            //string json_data = JsonUtility.ToJson(/*GenerateBoard.row*/ c);
            string json_data = JsonConvert.SerializeObject(/*GenerateBoard.row*/ c);
            File.WriteAllText(file_path, json_data);

            //return true;
            Debug.Log("Game Saved");
            Debug.Log(json_data);
            SaveGameTime();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void SaveGameTime()
    {
        string folder_path = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);
        string file_name = "game_time_data.json";
        string file_path = folder_path + "/" + file_name;

        string gt = gameTime.ToString();
        try
        {
            string json_data = JsonConvert.SerializeObject(/*GenerateBoard.row*/ gt);
            File.WriteAllText(file_path, json_data);

            //return true;
            Debug.Log("Game Time Saved");
            Debug.Log(json_data);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void LoadGameTime()
    {
        string folder_path = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);
        string file_name = "game_time_data.json";
        string file_path = folder_path + "/" + file_name;

        try
        {
            string json_data = File.ReadAllText(file_path);
            //Debug.Log(float.Parse(json_data.Trim('"')));
            gameTime = float.Parse(json_data.Trim('"'));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }


    public void LoadGame()
    {
        string folder_path = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);
        string file_name = "game_data.json";
        string file_path = folder_path + "/" + file_name;

        try
        {
            string json_data = File.ReadAllText(file_path);
            ValuesToSaveCasilla[,] game_data = new ValuesToSaveCasilla[9, 9];
            game_data = JsonConvert.DeserializeObject<ValuesToSaveCasilla[,]>(json_data);
            //var game_data = JsonUtility.FromJson<Casilla[,]>(json_data);

            //return game_data;

            //Load values to board
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    GenerateBoard.row[x, y].valueText.text = "";

                    GenerateBoard.row[x, y].value = game_data[x, y].value;
                    GenerateBoard.row[x, y].isWritable = game_data[x, y].isWritable;
                    if(!game_data[x, y].isWritable)
                        GenerateBoard.row[x, y].SetFeatures(arial);
                    else
                        GenerateBoard.row[x, y].SetFeatures(homemadeApple);

                    if(game_data[x, y].value != 0)
                        GenerateBoard.row[x, y].valueText.text = game_data[x, y].value.ToString();

                }
            }
            //Check board if is OK
            CheckBoard();
            //
            //Load time
            LoadGameTime();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }


        //return null;
    }

    public void DeleteSaveGame()
    {
        string folder_path = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath);
        string file_game_time_name = "game_time_data.json";
        string file_game_data_name = "game_data.json";
        string file_game_time_path = folder_path + "/" + file_game_time_name;
        string file_game_data_path = folder_path + "/" + file_game_data_name;

        if (File.Exists(file_game_data_path))
        {
            File.Delete(file_game_data_path);
            Debug.Log("Game data deleted!!!");
        }

        if (File.Exists(file_game_time_path))
        {
            File.Delete(file_game_time_path);
            Debug.Log("Game time deleted!!!");
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitGameInSeconds(float seconds)
    {
        Debug.Log($"Quit in {seconds} seconds");
        Time.timeScale = 1;
        Invoke("ExitGame", seconds);
    }

    public void FadeOutImage(float time)
    {
        if (blackScreenImage != null)
        {
            blackScreenImage.color = new Color(0, 0, 0, 1);
            blackScreenImage.CrossFadeAlpha(0, time, true);
        }
            
    }

    public void FadeInImage(float time)
    {
        if (blackScreenImage != null)
        {
            blackScreenImage.color = new Color(0, 0, 0, 1);
            blackScreenImage.CrossFadeAlpha(1, time, true);
        }

    }
}


