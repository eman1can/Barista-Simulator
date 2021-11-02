using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * A weather control system. For now it only holds the ambient temperature
 * But in future will be expanded to include rain / clouds and a fluctuating ambient temp
 */
public class WeatherController : MonoBehaviour {
    // For now, Ambient room temp will always be 20°
    private float ambientTemperature = 20f;

    public float GetAmbientTemperature() {
        return ambientTemperature;
    }
}
