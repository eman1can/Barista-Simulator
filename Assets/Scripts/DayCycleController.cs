using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Created by following video by Dominik Hackl
 * https://www.youtube.com/watch?v=H3JpkcGi8DI&t=1011s
 *
 * Created a day and night manager to control the sun and moon directional lights
 */
public class DayCycleController : MonoBehaviour {
    [Tooltip("Enable Day/Night Cycle")] public bool isCycleEnabled;
    
    [Range(0, 24)] public float timeOfDay;

    public Light sun;
    public Light moon;
    public float orbitSpeed = 1.0f;
    private bool isNight;
    
    void Start() { }

    void Update() {
        if (isCycleEnabled) {
            timeOfDay += Time.deltaTime * orbitSpeed;
            if (timeOfDay > 24)
                timeOfDay -= 24;
            updateTime();
        }
    }

    private void OnValidate() {
        updateTime();
    }

    private void updateTime() {
        var alpha = timeOfDay / 24.0f;
        var sunRotation = Mathf.Lerp(-90, 270, alpha);
        var moonRotation = sunRotation - 180;
        sun.transform.rotation = Quaternion.Euler(sunRotation, -150.0f, 0);
        moon.transform.rotation = Quaternion.Euler(moonRotation, -150.0f, 0);
        CheckNightlyTransition();
    }

    private void CheckNightlyTransition() {
        if (isNight) {
            if (moon.transform.rotation.eulerAngles.x > 180) {
                StartDay();
            }
        } else {
            if (sun.transform.rotation.eulerAngles.x > 180) {
                StartNight();
            }
        }
    }

    private void StartDay() {
        isNight = false;
        sun.shadows = LightShadows.Soft;
        sun.gameObject.SetActive(true);
        moon.shadows = LightShadows.None;
        moon.gameObject.SetActive(false);
    }

    private void StartNight() {
        isNight = true;
        sun.gameObject.SetActive(false);
        sun.shadows = LightShadows.None;
        moon.shadows = LightShadows.Soft;
        moon.gameObject.SetActive(true);
    }
}