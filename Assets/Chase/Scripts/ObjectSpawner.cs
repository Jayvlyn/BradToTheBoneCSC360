using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Bounds spawnArea;
    [SerializeField] private List<SpawnableItem> itemsToSpawn;
    [SerializeField] private int amountToSpawn;
    [SerializeField] private float collisionCheckRadius;
    [SerializeField] private bool spawnOnLoad;
    [SerializeField] private bool useWeightAsCount;

    public void StartSpawning() {
        float totalWeight = 0;
        foreach (SpawnableItem item in itemsToSpawn) {
            totalWeight += item.weight;
        }
        if(!useWeightAsCount) {
            for(int i = 0; i < amountToSpawn; i++) {
                SpawnObject(totalWeight);
            }
        }
        else {
            SpawnObject(itemsToSpawn.Count);
        }
    }
    private void Start() {
        if(spawnOnLoad) StartSpawning();
    }
    private void SpawnObject(float totalWeight) {
        if(!useWeightAsCount) {
            float weight = Random.Range(0, totalWeight);
            foreach (SpawnableItem item in itemsToSpawn) {
                weight -= item.weight;
                if(weight <= 0) {
                    int iteration = 0;
                    Vector2 spawnPoint = new Vector2(Random.Range(spawnArea.min.x, spawnArea.max.x),Random.Range(spawnArea.min.y, spawnArea.max.y)) + (Vector2)transform.position;
                    while(!(!ObjectsInRange(spawnPoint, collisionCheckRadius) || iteration >= 20)) {
                        //Debug.Log("Overlap occurred");
                        iteration++;
                        spawnPoint = new Vector2(Random.Range(spawnArea.min.x, spawnArea.max.x),Random.Range(spawnArea.min.y, spawnArea.max.y)) + (Vector2)transform.position;
                    }
                    if(iteration >= 20) Debug.Log("One has to collide after 20 iteration");
                    Instantiate(item.prefab, spawnPoint, Quaternion.identity);
                    return;
                }
            }
        }
        else {
            foreach(SpawnableItem item in itemsToSpawn) {
                for(int i = 0; i < item.weight; i++) {
                    int iteration = 0;
                    Vector2 spawnPoint = new Vector2(Random.Range(spawnArea.min.x, spawnArea.max.x),Random.Range(spawnArea.min.y, spawnArea.max.y)) + (Vector2)transform.position;
                    while(!(!ObjectsInRange(spawnPoint, collisionCheckRadius) || iteration >= 20)) {
                        //Debug.Log("Overlap occurred");
                        iteration++;
                        spawnPoint = new Vector2(Random.Range(spawnArea.min.x, spawnArea.max.x),Random.Range(spawnArea.min.y, spawnArea.max.y)) + (Vector2)transform.position;
                    }
                    if(iteration >= 20) Debug.Log("One has to collide after 20 iteration");
                    Instantiate(item.prefab, spawnPoint, Quaternion.identity);
                }
            }
        }
    }
    private bool ObjectsInRange(Vector2 point, float radius) {
        bool toClose = false;
        foreach(GameObject e in FindGameObjectsWithLayer(6)) {
            if(Vector2.Distance(point, (Vector2)e.transform.position) <= radius) toClose = true;
        }
        return toClose;
    }
    private GameObject[] FindGameObjectsWithLayer(int layer) {
    	var goArray = FindObjectsOfType<GameObject>();
    	var goList = new System.Collections.Generic.List<GameObject>();
    	for (var i = 0; i < goArray.Length; i++) {
    		if (goArray[i].layer == layer) {
 			goList.Add(goArray[i]);
 		}
 	}
 	return goList.ToArray();
}




    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(spawnArea.center + transform.position, spawnArea.size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnArea.center + transform.position, collisionCheckRadius);
    }
}

[System.Serializable]
public class SpawnableItem
{
    public float weight;
    public GameObject prefab;
}