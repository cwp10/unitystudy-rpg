using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    public static ObjectPool instance;
    public GameObject[] objectPrefabs;
    public List<GameObject>[] pooledObjects;
    public int[] amountToBuffer;
    int defaulBufferAmount = 3;
    protected GameObject containerObject;

    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        containerObject = new GameObject("ObjectPool");
        pooledObjects = new List<GameObject>[objectPrefabs.Length];
        int i = 0;
        foreach (GameObject objectPrefab in objectPrefabs) {
            int bufferAmount;
            pooledObjects[i] = new List<GameObject>();

            if (i < amountToBuffer.Length) {
                bufferAmount = amountToBuffer[i];
            } else {
                bufferAmount = defaulBufferAmount;
            }
            i++;
            for (int n = 0; n < bufferAmount; n++) {
                GameObject newObj = Instantiate(objectPrefab) as GameObject;
                newObj.name = objectPrefab.name;
                PoolObject(newObj);
            }
        }
	}

    public GameObject GetObjectForType(string objectType) {
        for (int i = 0; i < objectPrefabs.Length; i++) {
            GameObject prefab = objectPrefabs[i];

            if (prefab != null && prefab.name == objectType) {
                if (pooledObjects[i].Count > 0) {
                    GameObject pooledObject = pooledObjects[i][0];
                    pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.SetParent(null);
                    pooledObject.SetActive(true);
                    return pooledObject;
                }
                break;
            }
        }
        return null;
    }

    public void PoolObject(GameObject obj) {
        for (int i = 0; i < objectPrefabs.Length; i++) {
            if (objectPrefabs[i].name == obj.name) {
                obj.SetActive(false);
                obj.transform.parent = containerObject.transform;
                pooledObjects[i].Add(obj);
                return;
            }
        }
    }
}
