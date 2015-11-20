using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerUI : MonoBehaviour {

    public GameObject prefab;
    List<GameObject> listObject = new List<GameObject>();

    void OnGUI() {
        if (GUI.Button(new Rect(10, 210, 100, 50), "Spawn")) {
            GameObject obj = ObjectPool.instance.GetObjectForType(prefab.name);
            if (obj != null) {
                listObject.Add(obj);
                float x = Random.Range(-10.0f, 10.0f);
                float z = Random.Range(-10.0f, 10.0f);
                obj.transform.position = new Vector3(x, obj.transform.position.y, z);
            }
        }

        if (GUI.Button(new Rect(10, 310, 100, 50), "Disable")) {
            if (listObject.Count > 0) {
                ObjectPool.instance.PoolObject(listObject[0]);
                listObject.RemoveAt(0);
            }
        }
    }
}
