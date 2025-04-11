using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surfboard : BeachObject, IKickable
{
    [SerializeField] private List<Sprite> spriteVariations = new List<Sprite>();
    [SerializeField] private float powerModifier;
    [SerializeField] private float time;
    [SerializeField] private float offset;
    public bool OnKicked(Player player) {
        if(state == ObjectState.Complete) {
            state = ObjectState.Moving;
            StartCoroutine(KickTo(player, time));
            //playAudio();
            return true;
        }
        return false;
    }
    public void Dusted(Player player) {
        if(state == ObjectState.Complete) {
        }
    }
    private void Start() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = spriteVariations[Random.Range(0,3)];
        sr.color = new Color(Random.Range(0.5f, 1.0f),Random.Range(0.5f, 1.0f),Random.Range(0.5f, 1.0f));
    }
    
    IEnumerator KickTo(Player player, float time) {
        playAudio();
        float elapsedTime = 0;
        Vector2 startPos = transform.position;
        Vector2 endPos = (Vector2)player.transform.position + (player.rb.velocity.normalized * ((player.rb.velocity.magnitude * powerModifier)));
        while(elapsedTime < time) {
            elapsedTime += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, endPos, elapsedTime/time);
            transform.rotation = Quaternion.Euler(0, 0, (elapsedTime / time) * 360);
            yield return null;
        }
        state = ObjectState.Complete;

        transform.position = endPos;
        transform.rotation = Quaternion.identity;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        BeachObject b = other.gameObject.GetComponent<BeachObject>();
        if(b != null && b.surfCanDestroy) {
            
            b.GetComponent<IKickable>().Dusted(FindAnyObjectByType<Player>());
        }
            
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, new Vector2(0, -offset));
    }
}


// surfboard not go on water destroy
// surfboard destroy and gain points for all
// graphic on kick thing
//