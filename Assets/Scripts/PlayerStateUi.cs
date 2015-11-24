using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStateUi : MonoBehaviour {

    public Image[] uiSlider = null;

    public enum SLIDERTYPE {
        HP = 0,
        MP
    }

    Color hpBaseColor = Color.white;
    Image hpWidget = null;

    void Start() {
        hpWidget = uiSlider[(int)SLIDERTYPE.HP];
        hpBaseColor = hpWidget.color;

        foreach (Image slider in uiSlider) {
            slider.fillAmount = 1.0f;
        }
    }

    public void UpdateBar(SLIDERTYPE type, float value) {
        UpdateBar(type, uiSlider[(int)type], value);
    }

    void UpdateBar(SLIDERTYPE type, Image slider, float value) {

        if (type == SLIDERTYPE.HP) {
            UpdateHpBar(value);
        } else if (type == SLIDERTYPE.MP) {
            UpdateMpBar(value);
        }
    }

    void UpdateHpBar(float value) {
        uiSlider[(int)SLIDERTYPE.HP].fillAmount = value;
        hpWidget.color = Color.Lerp(Color.red, hpBaseColor, value);
    }

    void UpdateMpBar(float value) {
    }
}
