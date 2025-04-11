using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BradEndMoving : MonoBehaviour
{
    public Image brad;
    public Image background;
    public Image wall;
    public Canvas textCanvas;

    public Transform start;
    public Transform middle;
    public Transform end;


    private bool startMovement = false;

    // Start is called before the first frame update
    private void Start() {
        StartMove();
    }

    void StartMove()
    {
        textCanvas.sortingOrder = 100;
        background.canvas.sortingOrder = 0;
        wall.canvas.sortingOrder = 2;
        brad.canvas.sortingOrder = 1;

        transform.position = start.position;

        startMovement = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(startMovement) {
            transform.position = Vector2.Lerp(transform.position, end.position, 1 * Time.deltaTime);
            if (transform.position.x > middle.position.x)
            {
                brad.canvas.sortingOrder = 5;
            }
        }
    }
}
