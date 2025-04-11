using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class ScoreManager : MonoBehaviour
{
    public ScriptableValue score;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string textBefore;
    [SerializeField] private bool resetOnLoad;
    [SerializeField] private bool amBestScore;
    public float buildupScore = 0;
    [SerializeField] private ScriptableValue bestScore;

    private float elapsedTime = 0.0f;
    private float countUpTime = 7.775f;

    private void Start() 
    {
        if(resetOnLoad) score.value = 0;

        if (amBestScore)
        {
            if (!PlayerPrefs.HasKey("BestScore"))
            {
                PlayerPrefs.SetInt("BestScore", score.value);
                Load();
            }
            else
            {
                if (score.value > PlayerPrefs.GetInt("BestScore"))
                {
                    Save();
                }
                Load();
            }
        }
    }

    public void Load()
    {
        bestScore.value = PlayerPrefs.GetInt("BestScore");
    }

    public void Save()
    {
        PlayerPrefs.SetInt("BestScore", score.value);
    }

    private void Update()
    {
        if (GameManager.GetActiveSceneName().Equals("GameOver"))
        {
            if (amBestScore)
            {
                if (bestScore.value - buildupScore < 100) buildupScore = bestScore.value;
                else
                {
                    buildupScore = (Mathf.Lerp(0, bestScore.value, elapsedTime/countUpTime));
                    elapsedTime += Time.deltaTime;
                    
                }
                scoreText.text = textBefore + (int)buildupScore;
            } 
            else
            {
                if (score.value - buildupScore < 100) buildupScore = score.value;
                else
                {
                    buildupScore = (Mathf.Lerp(0, score.value, elapsedTime/countUpTime));
                    elapsedTime += Time.deltaTime;
                }
                scoreText.text = textBefore + (int)buildupScore;
            }
        }
        else
        {
            if (amBestScore)
            {
                scoreText.text = textBefore + (int)bestScore.value;
            }
            else
            {
                scoreText.text = textBefore + (int)score.value;
            }
        }
    }
}
