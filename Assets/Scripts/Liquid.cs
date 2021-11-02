using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Defines a liquid, to be used in both thermodynamics as well
 * as interactions in the material
 */
public class Liquid : MonoBehaviour {
    [Header("Liquid Appearance Options")]
    [SerializeField, Tooltip("The Main Color of this object")]
    private Color sideColor;

    [SerializeField, Tooltip("The Main Color of this object")]
    private Color topColor;
    
    [Header("Fluid Dynamics options")]
    [SerializeField]
    private float decayConstant;

    private WeatherController _weatherController;
    private float _temperature;
    public void SetColors(Material material) {
        material.SetColor("_Tint", sideColor);
        material.SetColor("_TopColor", topColor);
    }
    private void Awake() {
        GameObject dayController = GameObject.Find("DayController");
        _weatherController = dayController.GetComponent<WeatherController>();
    }

    public float GetTemperature() {
        return _temperature;
    }

    public float GetSpecificHeat() {
        // Water
        return 4.19f;
    }
    
    public float GetHeatCapacity() {
        return 0.5918f;
    }
    public float GetDensity(float V) {
        return 0.001f / V;
    }
    // Viscosity Constant, µA = 2.414 × 10^−5
    // Viscosity Constant, µB = 247.8 K
    // Viscosity Constant, µC = 140 K
    // Viscosity, µ = µA * 10 ^ (µB / (T1 + 273.15 − µC)) Pa·s = 0.000311
    public float GetViscosity() {
        return 2.414f * Mathf.Pow(10, -5) * Mathf.Pow(10, 247.8f / (_temperature + 273.15f - 140f));
    }
    

    public void UpdateTemperature(float volume) {
        UpdateTemperature(volume, _weatherController.GetAmbientTemperature());
    }

    
    public void UpdateTemperature(float volume, float ambientTemp) {
        
        
        
        if (ambientTemp > _temperature) {
            // Equation 7 - T(t) = L - Ae ^ (-kt)
        } else {
            // Equation 8 - T(t) = L + Ae ^ (-kt)
        }
    }

    public void UpdateTemperature(float volume, float newVolume, float addedTemperature) {
        float addedVolume = newVolume - volume;
        
    }
}