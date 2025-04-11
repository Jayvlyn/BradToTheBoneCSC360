using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Wall : BeachObject, IKickable
{
    [SerializeField] private string sceneComplete;
    [SerializeField] private Image blackscreen;
    [SerializeField] private AudioSource wallBreak;
    [SerializeField] private AudioSource themeMusic;
    private TimerTide timerTide;
    private bool unlockWallBreak = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            unlockWallBreak = true;
        }
    }

    private void Start()
    {
        timerTide = FindAnyObjectByType<TimerTide>();
    }

    public void Dusted(Player player)
    {
        
    }

    public bool OnKicked(Player player)
    {
        
        return true;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log(timerTide.GetRemainingTime());
        if(timerTide.GetRemainingTime() < 10 && other.gameObject.CompareTag("Player") || unlockWallBreak)
        {
            StartCoroutine(GameEndTransition());
        }
    }

    private IEnumerator GameEndTransition()
    {
        blackscreen.gameObject.SetActive(true);
        GraphicManager.instance.graphics[0].SetActive(false);
        //playAudio();
        themeMusic.Stop();
        wallBreak.Play();
        

        // Wall break sound is 1.5 seconds 
        yield return new WaitForSeconds(1.5f);

        GameManager.SwitchScene(sceneComplete);
    }

}
