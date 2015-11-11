using UnityEngine;
using System.Collections;

public class Warrior : MonoBehaviour {

    Animator _animator;

    void Awake() {
        _animator = GetComponent<Animator>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.A)) {
            _animator.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            _animator.SetTrigger("hit");
        }

    }
}
