using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScene : MonoBehaviour
{
    public static bool isGamePaused = false;
    public void Pause(bool p)
    {
        Debug.Log("Pause method called");
        if (p)
        {
            isGamePaused = true;
            Time.timeScale = 0;
        }
        else
        {
            isGamePaused = false;
            Time.timeScale = 1;
        }
    }
}
