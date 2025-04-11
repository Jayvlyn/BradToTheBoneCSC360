using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Toggle fullscreenToggle;

    private void Awake()
    {
        if(GameManager.GetActiveSceneName().Equals("Kyle Scene"))
        {
            if (!PlayerPrefs.HasKey("screenToggle"))
            {
                PlayerPrefs.SetInt("screenToggle", 1);
                fullscreenToggle.isOn = (PlayerPrefs.GetInt("screenToggle") == 1);
            }
            else
            {
                if (PlayerPrefs.GetInt("screenToggle") == 1)
                {
                    Screen.SetResolution(1600, 900, false);
                }
                else if (PlayerPrefs.GetInt("screenToggle") == 0)
                {
                    Screen.SetResolution(1920, 1080, true);
                }

                fullscreenToggle.isOn = (PlayerPrefs.GetInt("screenToggle") == 0);
            }
        }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayGame();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayTutorial()
    {
        GameManager.SwitchScene("Tutorial");
    }

    public void QuitGame()
    {

        Application.Quit();
    }

    public void ToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ChangeResolution()
    {
        if(!fullscreenToggle.isOn)
        {
            PlayerPrefs.SetInt("screenToggle", 1);
            Screen.SetResolution(1600, 900, false);
        }
        else if (fullscreenToggle.isOn)
        {
            PlayerPrefs.SetInt("screenToggle", 0);
            Screen.SetResolution(1920, 1080, true);
        }

        print(PlayerPrefs.GetInt("screenToggle"));
    }


}
