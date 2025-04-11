using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using UnityEngine;

public class TimerTide : MonoBehaviour
{
    [SerializeField] private float gameTime = 100;
    [SerializeField] private Transform endPos;
    [SerializeField] private PolygonCollider2D cameraBounds;
    [SerializeField] private CinemachineConfiner2D cam;
    private float timer = 0;
    private Vector2 position;
    private bool hit10;

    private void Start() {
        position = transform.position;
    }
    private void Update() {



        timer += Time.deltaTime;
        float distance = transform.position.x - Vector2.Lerp(position, endPos.position, timer/gameTime).x;
        transform.position = Vector2.Lerp(position, endPos.position, timer/gameTime);
        
        Vector2[] path = {
            new Vector2 (cameraBounds.GetPath(0)[0].x - distance,cameraBounds.GetPath(0)[0].y),
            new Vector2 (cameraBounds.GetPath(0)[1].x - distance,cameraBounds.GetPath(0)[1].y),
            cameraBounds.GetPath(0)[2],
            cameraBounds.GetPath(0)[3]
            
        };
        cameraBounds.SetPath(0, path);
        cam.InvalidateCache();
        if(GetRemainingTime() <= 10 && !hit10) {
            hit10 = true;
            if(FindAnyObjectByType<Player>().moveState != Player.MoveState.Dead) GraphicManager.instance.graphics[0].SetActive(true);
        }
    }

    public float GetRemainingTime()
    {
        return gameTime - timer;
    }
}
