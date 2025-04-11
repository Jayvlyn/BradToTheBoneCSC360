using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urchin : BeachObject, IKickable
{
    [SerializeField] private int negativeValue;
    public void Dusted(Player player)
    {
        if(state == ObjectState.Complete) {
            destroySprite();
            UrchinPoints(false);
            state = ObjectState.Destroyed;
            playAudio(1);
        }
    }
    

    private void UrchinPoints(bool kicked) {
        int endV = 0;
        if(kicked) endV = (-negativeValue * 10);
        else {
            endV = (value * 10);
        }
        GraphicManager.instance.playGraphic(2,0.3f,transform.position, endV);
        scoreKeeper.add(endV);

    }

    public bool OnKicked(Player player)
    {
        if(state == ObjectState.Complete) {
            //Dusted();
            UrchinPoints(true);
            playAudio(0);
            return true;
        }
        return false;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Water")) {
            //Debug.Log("Hit Water");
            Destroy(this.gameObject, 1.0f);
        }
    }


}
