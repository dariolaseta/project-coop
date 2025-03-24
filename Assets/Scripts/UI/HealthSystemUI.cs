using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    
    public void UpdateHealthSlider(int value)
    {
        healthSlider.value = value;
    }
}
