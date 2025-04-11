using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawnDestroy : MonoBehaviour
{
    [SerializeField] private float aliveTime;
    [SerializeField] private float force;
    private void Start() {
        transform.rotation = Quaternion.Euler(Random.Range(0, 360),Random.Range(0, 360),Random.Range(0, 360));
        Destroy(this.gameObject, aliveTime);
        GetComponent<Rigidbody2D>().AddForce(new Vector2((Random.Range(-1, 1) * force),Random.Range(-1, 1) * force));
    }
}
