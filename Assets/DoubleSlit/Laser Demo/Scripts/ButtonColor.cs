using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonColor : MonoBehaviour {
    [SerializeField]
    private Oculus.Interaction.RoundedBoxProperties roundedBoxProperties;

    [SerializeField]
    private Color toggleOffColor;

    [SerializeField]
    private Color toggleOnColor;

    private bool toggle = false;

    void Start() {
        roundedBoxProperties.Color = toggleOffColor;
    }

    public void Toggle() {
        toggle = !toggle;
        print(toggle);
        if(toggle) {roundedBoxProperties.Color = toggleOnColor;} else {roundedBoxProperties.Color = toggleOffColor;}
        roundedBoxProperties.UpdateMaterialPropertyBlock();
    }
}
    
