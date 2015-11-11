using UnityEngine;
using System.Collections;

public class LegacySample1 : MonoBehaviour {

    public Animation animation;

	// Use this for initialization
	IEnumerator Start () {
        animation = GetComponent<Animation>();
        AnimationState idleState = animation["idle"];
        idleState.wrapMode = WrapMode.Loop;
        idleState.normalizedTime = 0.0f;
        idleState.speed = 1.0f;

        animation.Play("idle");

        yield return new WaitForSeconds(0.4f);	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A)) {
            animation.Play("attack_1");
            animation.PlayQueued("idle", QueueMode.CompleteOthers);
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            animation.Play("attack_2");
            animation.PlayQueued("idle", QueueMode.CompleteOthers);
        }
    }

    void OnAttack1() {
        AnimationState attackState = GetComponent<Animation>()["attack_1"];
        Debug.Log("attack_1" + attackState.length.ToString());
        Debug.Log("attack_1" + attackState.time.ToString());
    }
    void OnAttack2() {
        AnimationState attackState = GetComponent<Animation>()["attack_2"];
        Debug.Log("attack_2" + attackState.length.ToString());
        Debug.Log("attack_2" + attackState.time.ToString());
    }
}
