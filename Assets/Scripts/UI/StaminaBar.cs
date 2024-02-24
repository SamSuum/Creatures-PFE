using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StaminaBar : MonoBehaviour
{
    Slider _staminaSlider;
    private void Awake()
    {
        _staminaSlider = GetComponent<Slider>();
    }
    public void SetMaxStamina(int maxStamina)
    {
        _staminaSlider.maxValue = maxStamina;
        _staminaSlider.value = maxStamina;
    }
    public void SetStamina(int stamina)
    {
        _staminaSlider.value = stamina;
    }
}
