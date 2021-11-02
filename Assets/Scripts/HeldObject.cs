using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Used to hold info about an object and their type, for use in interaction
 */
public class HeldObject : MonoBehaviour {
    [SerializeField] private Transform centerPivot;
    [SerializeField] private string displayName;
    [SerializeField] protected HeldObjectType objectType;
    public enum HeldObjectType {
        EspressoMachineWaterContainer,
        EspressoMachinePortafilter,
        EspressoMachineDripTray,
        EspressoCup
    }
    public Transform GetCenterPivot() {
        return centerPivot;
    }

    public string GetName() {
        return displayName;
    }

    public HeldObjectType GetItemType() {
        return objectType;
    }

    public override bool Equals(object other) {
        return objectType == ((HeldObject) other).objectType;
    }
}