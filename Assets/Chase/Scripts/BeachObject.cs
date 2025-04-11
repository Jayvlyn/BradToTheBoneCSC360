using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachObject : MonoBehaviour
{
    [SerializeField] protected int value;
    [SerializeField] public ObjectState state;
    public bool surfCanDestroy;
    [SerializeField] protected Sprite destroyedState;
    [SerializeField] protected ScriptableValue scoreKeeper;
    [SerializeField] protected AudioSource[] gameAudio;

    protected void destroySprite() {
        state = ObjectState.Destroyed;
        GetComponent<SpriteRenderer>().sprite = destroyedState;
    }
    protected void playAudio() {
        foreach(AudioSource aud in gameAudio) {
            aud.Play();
        }
    }
    protected virtual void calculatePoints(Player player) {

    }
    protected void playAudio(int index) {
        
        gameAudio[index].Play();
    }
    
}

public enum ObjectState
{
    Complete,
    Destroyed,
    WashedAway,
    Moving
}