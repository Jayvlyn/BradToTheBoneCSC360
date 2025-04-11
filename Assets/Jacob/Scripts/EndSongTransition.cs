using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSongTransition : MonoBehaviour
{
    [SerializeField] private AudioSource tallySong;
    [SerializeField] private AudioSource finalSong;
    [SerializeField] private ScoreManager scoreManager;
    private bool finalSongPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        tallySong.Play();
        finalSongPlayed = false;
    }


    private void LateUpdate()
    {
        if (scoreManager != null)
        {
            //Debug.Log("Buildup finished: " + (scoreManager.buildupScore >= (scoreManager.score.value - 10.0f)));
            //Debug.Log("Buildup: " + (scoreManager.buildupScore));
            //Debug.Log("Score: " + (scoreManager.score.value));
            if ((scoreManager.buildupScore >= (scoreManager.score.value - 10.0f)) && !finalSong.isPlaying && !finalSongPlayed)
            {
                //Debug.Log("should play");
                finalSong.Play();
                finalSongPlayed=true;
            }
        }
    }
}
