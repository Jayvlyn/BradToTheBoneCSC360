using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;

    public GameObject title;
    public Transform titleStart;
    public Transform titleEnd;
    private float elapsedTime = 0.0f;
    private float animTime = 1.0f;

    private void Start()
    {
        elapsedTime = 0.0f;
        transform.position = startPos.position;
    }

    private void Update()
    {
        if(transform.position != endPos.position)
        {
            transform.position = Vector2.Lerp(transform.position, endPos.position, Time.deltaTime);
        }

        if(title.transform.position != titleEnd.position)
        {
            title.transform.position = Vector2.Lerp(titleStart.position, titleEnd.position, elapsedTime/animTime);
            elapsedTime += Time.deltaTime;
        }
    }
}
