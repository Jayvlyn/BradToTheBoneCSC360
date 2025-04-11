using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandCastle : BeachObject, IKickable
{
    [SerializeField] private float size;
    public GameObject kickAnim;
    public bool playSoundOnDown;

    public void Dusted(Player player)
    {
        if(state == ObjectState.Complete) {
            destroySprite();
            calculatePoints(player);
        }
    }
    protected override void calculatePoints(Player player) {
        int number = (int)(10 * value * (1.0f + (player.feetHeat * 0.25f / player.maxHeat)) * (player.moveState == Player.MoveState.Falling ? 2 : 1));
        scoreKeeper.add(number);
        GraphicManager.instance.playGraphic(2, 0.3f, transform.position, number);

    }

    public bool OnKicked(Player player)
    {
        
        if(state == ObjectState.Complete) {
            GameObject anim = Instantiate(kickAnim, this.transform);
            if(player.FacingRight())
            {
                anim.transform.localScale = new Vector3(-anim.transform.localScale.x, anim.transform.localScale.y, anim.transform.localScale.z);
            }
            playAudio();
            Dusted(player);
            
            return true;
        }
        else if(playSoundOnDown) {
            playAudio(0);
        }
        return false;
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Water")) {
            //Debug.Log("Hit Water");
            Destroy(this.gameObject, 2.0f);
        }
    }

}
