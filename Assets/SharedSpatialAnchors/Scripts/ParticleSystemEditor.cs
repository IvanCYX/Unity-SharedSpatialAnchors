using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParticleSystemEditor : MonoBehaviour
{
    public GameObject startSpeedSlider;

    public TextMeshPro startSpeedReadout;

    public ParticleSystem particleSystem;

    private float startSpeed = 15f;

    private Vector2 sliderRange = new Vector2(-0.095f, 0.095f);

    public bool sliderLock = true;

    [SerializeField]
    private uint sliderModified;

    private Vector3 centerPosition = new Vector3(-0.5f, 1.45f, 0.6f);

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startSpeed = startSpeed;
    }

    void Update()
    {
        if (particleSystem != null)
        {
            if (sliderLock) { determineSliderModified(); }
            calculateSliderValuesAfterModification();
            updateSpeedAndReadout();
            setStartSpeedSlider();
        }
    }

    private void updateSpeedAndReadout()
    {
        startSpeedReadout.text = decimal.Round((decimal)startSpeed, 2).ToString();
    }

    //setting slider range to interface particle sys startSpeed
    private void setStartSpeedSlider()
    {
        setSliderToPosition(startSpeedSlider, remap(Constants.minimumStartSpeed, Constants.maximumStartSpeed,
                                                           sliderRange.x, sliderRange.y,
                                                           startSpeed));
    }

    private void updateSliders()
    {
        setStartSpeedSlider();
    }

    private float readStartSpeedSlider()
    {
        return remap(sliderRange.x, sliderRange.y,
                     Constants.minimumStartSpeed, Constants.maximumStartSpeed,
                     readSliderPosition(startSpeedSlider));
    }

    private void calculateSliderValuesAfterModification()
    {
        switch (sliderModified)
        {
            case 0: //No slider has been moved
                return;
            case 1: //Refractive index slider has been moved
                startSpeed = readStartSpeedSlider();
                return;
        }
    }

    private void determineSliderModified()
    {
        sliderModified = 1;

    }

    private void setSliderToPosition(GameObject slider, float position)
    {
        slider.transform.localPosition = new Vector3(position, slider.transform.localPosition.y, slider.transform.localPosition.z);
    }


    private float readSliderPosition(GameObject slider)
    {
        return slider.transform.localPosition.x;
    }


    private float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
    {
        float rel = Mathf.InverseLerp(origFrom, origTo, value);
        return Mathf.Lerp(targetFrom, targetTo, rel);
    }
}
