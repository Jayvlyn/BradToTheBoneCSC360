using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphicManager : MonoBehaviour
{
    public static GraphicManager instance;
    private void Awake() {
        instance = this;
        foreach(GameObject e in graphics) {
            e.SetActive(false);
        }
    }
    [SerializeField] public GameObject[] graphics;
    private GameObject activeGraphic;

    public void playGraphic(int index, float time) {
        if(activeGraphic != null) activeGraphic.SetActive(false);
        activeGraphic = graphics[index];
        activeGraphic.SetActive(true);
        StartCoroutine(StopAfterTime(time));
    }
    public void playGraphic(int index, float time, Vector2 place, int value) {
        playGraphic(index, time);
        //activeGraphic.transform.localPosition = new Vector3(place.x - 200, place.y - 50, 0);
        float scale = (1.0f + ((Mathf.Abs(value) - 0.3f) / (1000.0f - 0.3f))) * 0.4f;
        scale = Mathf.Clamp(scale, 0.6f, 1.6f);
        activeGraphic.transform.localScale = new Vector3(scale, scale, 1);
        activeGraphic.GetComponent<TMP_Text>().text = "" + value;

        // Set color of text
        if(value > 0)activeGraphic.GetComponent<TMP_Text>().color = new Color(0, 255, 0, 255);
        else         activeGraphic.GetComponent<TMP_Text>().color = new Color(255, 0, 0, 255);

        activeGraphic.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));
    }
    IEnumerator StopAfterTime(float time) {
        yield return new WaitForSeconds(time);
        activeGraphic.SetActive(false);

    }
}
