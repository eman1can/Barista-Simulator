using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Controls portafilter information
 * Will eventually control visual state too
 */
public class Portafilter : MonoBehaviour {
    private bool hasGrounds = false;
    private bool groundsTamped = false;
    private bool groundsSpoiled = false;
    
    public bool HasGrounds => hasGrounds;
    public bool GroundsTamped => groundsTamped;
    public bool GroundsSpoiled => groundsSpoiled;

    public void AddGrounds() {
        hasGrounds = true;
    }

    public void TampGrounds() {
        groundsTamped = true;
    }

    public void DumpGrounds() {
        groundsSpoiled = false;
        hasGrounds = false;
        groundsTamped = false;
    }

    public void SpoilGrounds() {
        groundsSpoiled = true;
    }
}