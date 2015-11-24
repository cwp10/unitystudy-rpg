using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITest : MonoBehaviour {

    public Text txt;
    public InputField inputfield;

	// Use this for initialization
	void Start () {
   
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /*public void OnTestButton(int arg) {
        Debug.Log("Test :" + arg);
    }*/

    public void OnTestButton() {
        txt.text = inputfield.text;
    }
}
