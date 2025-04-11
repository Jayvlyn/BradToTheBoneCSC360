using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinker : MonoBehaviour
{
    [SerializeField] private Image image;
    public bool blink;
    [SerializeField] private float time;
    private float elapsedTime;

    private void Update() {
        if(blink) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, Mathf.Abs(elapsedTime)/time));
            elapsedTime -= Time.deltaTime;
            if(elapsedTime <= -time) elapsedTime = time;
        }
        else image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
}
