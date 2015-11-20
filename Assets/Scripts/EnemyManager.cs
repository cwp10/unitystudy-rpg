using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

    public GameObject enemyPrefab = null;
    float deltaTime = 0.0f;
    public float spawnTime = 2.0f;
	
	// Update is called once per frame
	void Update () {
        deltaTime += Time.deltaTime;

        if (deltaTime > spawnTime) {
            deltaTime = 0.0f;
            GameObject obj = ObjectPool.instance.GetObjectForType(enemyPrefab.name);

            if (obj != null) {
                float x = Random.Range(-10.0f, 10.0f);
                float z = Random.Range(-10.0f, 10.0f);
                obj.transform.position = new Vector3(x, obj.transform.position.y, z);
            }
        }
	}
}
