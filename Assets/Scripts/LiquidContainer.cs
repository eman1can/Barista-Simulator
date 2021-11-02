using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Used to define a liquid container, and holds info for both thermodynamics, the material and interaction triggers
 * I'm pretty certain I wrote all this code myself, but it references the liquid shader, which I did not code myself
 */
public class LiquidContainer : MonoBehaviour {
    [Header("Material Options")]
    [SerializeField, Tooltip("The Liquid Mesh that has the Liquid Material applied")]
    private GameObject liquid;
    [SerializeField, Tooltip("The distance to the bottom of the clipping zone")]
    private float clipBottom = -0.1f;
    [SerializeField, Tooltip("The distance to the top of the clipping zone")]
    private float clipTop = 0.1f;
    
    [Header("Container Options")]
    // Size is in ml
    [Tooltip("The Amount of liquid to hold in ml")]
    public float size = 1000f;
    [SerializeField, Tooltip("The Amount of liquid currently held")]
    protected float _amount = 0f;
    
    // Container sizes
    //private float containerVolume = 0f;
    //private float outsideAreaBottom = 0f;
    //private float outisdeAreaSide1 = 0f;
    //private float outisdeAreaSide2 = 0f;
    //private float insideAreaBottom = 0f;
    //private float insideAreaSide1 = 0f;
    //private float insideAreaSide2 = 0f;
    //private float thickness = 0f;

    private float materialHeatCapacity = 0f;
    private float materialEmissivity = 0f;
    
    // The reference to the liquid material
    private Material _material;
    // The fill percent to be passed to the material
    private float _fillPercent = 0f;
    private int _fillPercentID = Shader.PropertyToID("_FillAmount");

    // The liquid that is currently stored
    private Liquid _liquid;
    private float _volume;
    
    // This container has heat applied
    // E.g Coffee maker, heat plate
    private bool _tempApplied;
    private float _temperature;
    private void Awake() {
        Renderer renderer = liquid.GetComponent<Renderer>();
        _material = renderer.material;
        _material.SetFloat("_Min", clipBottom);
        _material.SetFloat("_Max", clipTop);

        Vector3 size = renderer.bounds.size;
        _volume = size.x * size.y * size.z;
    }

    public void UpdateMaterial(Renderer renderer, Material material) {
        renderer.material = material;
        _material = material;
        _material.SetFloat("_Min", clipBottom);
        _material.SetFloat("_Max", clipTop);
    }

    private void Update() {
        _fillPercent = _amount / size;
        float volume = _volume * _fillPercent;
        if (_liquid != null) {
            if (_tempApplied)
                _liquid.UpdateTemperature(volume, _temperature);
            else
                _liquid.UpdateTemperature(volume);
        }
    }

    public void SetLiquid(Liquid liquid) {
        liquid.SetColors(_material);
        _liquid = liquid;
    }

    public GameObject GetLiquidObject() {
        return liquid;
    }

    public bool IsFilled() {
        return _amount >= size;
    }

    public float FillPercent() {
        return _fillPercent;
    }

    public float GetFillAmount() {
        return _amount;
    }

    public float GetFillMax() {
        return size;
    }

    public void AddLiquid(float amount, float temperature) {
        float volume = _volume * _fillPercent;
        _amount += amount;
        updatePercent();
        float newVolume = _volume * _fillPercent;
        _liquid.UpdateTemperature(volume, newVolume, temperature);
    }

    public void RemoveLiquid(float amount) {
        _amount -= amount;
        if (_amount < 0)
            _amount = 0;
        updatePercent();
    }

    public void updatePercent() {
        _fillPercent = _amount / size;
        _material.SetFloat(_fillPercentID, _fillPercent);
        Debug.Log(_amount + " - " + size + " - " + _fillPercent);
    }

    public void OnValidate() {
        if (_material != null) {
            _material.SetFloat("_Min", clipBottom);
            _material.SetFloat("_Max", clipTop);
            _material.SetFloat(_fillPercentID, _fillPercent);
        }
    }
}
