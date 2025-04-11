using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : BeachObject, IKickable
{
    [SerializeField] private ObjectSpawner spawner;
    [SerializeField] private ObjectSpawner roofPiecesSpawner;
    public bool OnKicked(Player player)
    {
        if(state == ObjectState.Complete) {
            calculatePoints(player);
            
            Dusted(player);
            return true;
        }
        return false;
    }
    public void Dusted(Player player) {
        if(state == ObjectState.Complete) {
            destroySprite();
            spawner.StartSpawning();
            roofPiecesSpawner.StartSpawning();
            calculatePoints(player);
            playAudio();
        }
    }
    protected override void calculatePoints(Player player) {
        scoreKeeper.add(value * 10);
        GraphicManager.instance.playGraphic(2,0.3f,transform.position, value * 10);
    }

    
}
