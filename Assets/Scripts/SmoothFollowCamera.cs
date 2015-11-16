using UnityEngine;
using System.Collections;

public class SmoothFollowCamera : MonoBehaviour {

    Transform _transform;
    public Transform targetTransform;
    public float distance = 10.0f;
    public float height = 5.0f;
    public float heightDamping = 2.0f;

    Vector3 lastForward = Vector3.forward;
    Vector3 toForward = Vector3.forward;
    public float smoothChange = 1.0f;

    public float newDistance = 0.0f;
    public float changeSpeedForDistance = 50.0f;

    public float targetChangeTime = 0.5f;
    float zoomVelocity = 0.0f;
    
    void Awake() {
        _transform = transform;
    }
	
	void LateUpdate () {
        if (targetTransform == null) {
            return;
        }

        float wantedHeight = targetTransform.position.y + height;
        float currentHeight = transform.position.y;

        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f) {
            newDistance = distance + -Input.GetAxis("Mouse ScrollWheel") * changeSpeedForDistance;
        }

        if (newDistance != distance) {
            distance = Mathf.SmoothDamp(distance, newDistance, ref zoomVelocity, targetChangeTime);
        }

        _transform.position = targetTransform.position;

        if (Input.GetButtonDown("Fire2")) {
            toForward = targetTransform.forward;
        }

        lastForward = Vector3.Lerp(lastForward, toForward, smoothChange * Time.deltaTime);
        _transform.position -= lastForward * distance;

        Vector3 cameraPos = _transform.position;
        cameraPos.y = currentHeight;
        _transform.position = cameraPos;

        transform.LookAt(targetTransform);
    }
}
