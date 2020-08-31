using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBar : MonoBehaviour
{

    public static SpeedBar instance;

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public static float amount;


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        StartCoroutine(Count());
        amount = slider.minValue;
    }


    private void SetSlider(float amount)
    {
        if (amount > slider.maxValue) amount = slider.maxValue;

        fill.color = gradient.Evaluate((amount - slider.minValue) / (slider.maxValue - slider.minValue));
        slider.value = amount;
    }


    public void StartCount()
    {
        amount = slider.minValue;
        SetSlider(amount);
        StartCoroutine(Count());
    }


    public void StopCount()
    {
        StopAllCoroutines();
        Player.instance.ballSpeed = amount;
    }


    private IEnumerator Count()
    {
        while (true)
        {
            amount += slider.maxValue * 0.6f * Time.fixedDeltaTime;
            if (amount > slider.maxValue) amount = slider.minValue;

            SetSlider(amount);
            
            yield return new WaitForFixedUpdate();
        }
    } 
}
