using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject objectToFollow;
    private void Update() {
        transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, -10);
    }
}
