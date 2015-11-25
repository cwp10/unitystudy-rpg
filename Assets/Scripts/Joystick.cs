using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour {

    Warrior warrior = null;
    bool _isDown = false;
    Vector3 touchStartPosition = Vector3.zero;

    public float radius = 40.0f;
    public Camera mainCamera;

	// Use this for initialization
	void Start () {
        warrior = GameObject.FindGameObjectWithTag("Player").GetComponent<Warrior>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) {
            OnPress(true);
        }

        if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended) {
            OnPress(false);
        }

        if (_isDown == false) {
            return;
        }

#if UNITY_EDITOR
        Vector3 currentPos = Input.mousePosition;

#else
        Vector3 currentPos = Vector3.zero;
        foreach (Touch currentTouch in Input.touches) {
            if (currentTouch.fingerId == lastTouchIndex) {
                currentPos = currentTouch.position;
            }
        }  
#endif
        Vector3 gap = currentPos - touchStartPosition;
        transform.localPosition = Vector3.zero + gap;

        float length = transform.localPosition.magnitude;

        if (length > radius) {
            transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, radius);
        }

        Vector3 moveForward = new Vector3(gap.x, 0.0f, gap.y);
        moveForward = mainCamera.transform.TransformDirection(moveForward);
        moveForward.y = 0.0f;

        float currentLength = moveForward.magnitude / radius;
        float ratio = Mathf.Clamp(currentLength, 0.0f, 1.0f);

        moveForward.Normalize();
        moveForward *= ratio;

        if (moveForward.Equals(Vector3.zero) == false) {
            warrior.Move(moveForward);
        }
	}

    int lastTouchIndex = -1;
    void OnPress(bool isDown) {
        _isDown = isDown;

        if (_isDown) {
            warrior.autoAttack = false;
#if UNITY_EDITOR
            touchStartPosition = Input.mousePosition;
#else
            foreach (Touch currentTouch in Input.touches) {
                if (currentTouch.phase == TouchPhase.Began) {
                    lastTouchIndex = currentTouch.fingerId;
                    touchStartPosition = currentTouch.position;
                    break;
                }
            }
#endif 
        } else {
            warrior.autoAttack = true;
            transform.localPosition = Vector3.zero;
            warrior.SetIdle();
        }
    }
}
