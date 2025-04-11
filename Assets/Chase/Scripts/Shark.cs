using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform ocean;
    [SerializeField] private float timeBeforeFollow;
    [SerializeField] private float speed;
    [SerializeField] private float circleScale;

    private float playerInWaterTimer;
    private bool playerEnteredWater = false;
    void Update()
    {
        if(!playerEnteredWater && player.onWater) {
            playerEnteredWater = true;
            playerInWaterTimer = 0;
        }
        if(playerEnteredWater) {
            playerInWaterTimer += Time.deltaTime;
            if(playerInWaterTimer >= timeBeforeFollow) {
                transform.position += (player.transform.position - transform.position).normalized * speed * Time.deltaTime;
            }
        }
        else {
            Vector3 target = new Vector2(ocean.position.x + (Mathf.Cos(Time.time) * circleScale), ocean.position.y + (Mathf.Cos(Time.time) * 2 * circleScale));
            transform.position += (target - transform.position).normalized * speed * Time.deltaTime;
        }

        if(playerEnteredWater && !player.onWater) {
            playerEnteredWater = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            other.gameObject.GetComponent<Player>().ChangeState(Player.MoveState.Dead);
            StartCoroutine(player.WaitToEnd());
        }
    }
}
