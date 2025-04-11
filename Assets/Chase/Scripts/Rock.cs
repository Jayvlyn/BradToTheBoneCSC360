using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : BeachObject, IKickable
{
    [SerializeField] private bool destroyable;
    public void Dusted(Player player)
    {
        destroySprite();
    }

    public bool OnKicked(Player player)
    {
        if(state == ObjectState.Complete && destroyable) {
            Dusted(player);
            return true;
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
