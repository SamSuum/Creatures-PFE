using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PsyBar : MonoBehaviour
{
    Slider _psySlider;
    private void Awake()
    {
        _psySlider = GetComponent<Slider>();
    }
    public void SetMaxPsy(int maxPsy)
    {
        _psySlider.maxValue = maxPsy;
        _psySlider.value = maxPsy;
    }
    public void SetPsy(int psy)
    {
        _psySlider.value = psy;
    }
}
