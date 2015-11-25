using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiHudBar : MonoBehaviour {

    public Transform targetTransform = null;
    Transform myTransform = null;
    public float offsetHeight = 0.6f;
    public Image slider = null;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        myTransform.position = targetTransform.position + Vector3.up * offsetHeight;
    }

    public void UpdateHpBar(float value) {
        slider.fillAmount = value;
    }

    void OnEnable() {
        UpdateHpBar(1.0f);
    }
}
