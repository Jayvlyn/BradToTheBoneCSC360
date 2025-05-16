using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public bool paused = false;
    public Canvas pauseCanvas;

    private void Start()
    {
        pauseCanvas.enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused) 
            { 
                paused = true;
                Time.timeScale = 0.0f;
                pauseCanvas.enabled = true;
            }
            else
            {
                paused = false;
                Time.timeScale = 1.0f;
                pauseCanvas.enabled = false;
            } 
        }
    }

    public void Resume()
    {
        paused = false;
        pauseCanvas.enabled = false;
        Time.timeScale = 1.0f;
    }

    public void QuitToMenu()
    {
        paused = false;
        Time.timeScale = 1.0f;
        GameManager.Instance.SwitchScene("Kyle Scene");
    }
}
