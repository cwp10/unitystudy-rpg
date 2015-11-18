using UnityEngine;
using System.Collections;

public class MessageTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnDamage() {
        Debug.Log("OnDamage was called!");
    }
}
