using UnityEngine;
using System.Collections;

public class SmoothFollowCamera_TR : MonoBehaviour {

    public Transform target;
    public float dampRotate = 5f;
    public Transform dest;

    Transform tr;

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        //tr.position = dest.position;
        Vector3 pos = Vector3.Lerp(tr.position, dest.position, dampRotate * Time.deltaTime);

        tr.position = pos;
        tr.LookAt(target);
	}
}
