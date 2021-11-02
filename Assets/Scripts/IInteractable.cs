/*
 * Base class created by following video by Dapper Dino
 * https://www.youtube.com/watch?v=saM9D1V6uNg
 *
 * Extended heavily since then to include outlining and popups.
 * Outline code functions on the outline custom pass shader found at
 * https://github.com/alelievr/HDRP-Custom-Passes
 *
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using static Utils;
public class IInteractable : MonoBehaviour {
    [SerializeField] protected RadialInteraction interaction;
    [SerializeField] protected InteractionController interactionGUI;
    [SerializeField] protected String displayName;
    [SerializeField] protected Sprite[] images;
    [SerializeField] protected string[] infoStrings;
    [SerializeField] protected int infoCount = 0;
    
    protected Layers currentLayer = Layers.Selection;
    
    [Header("Interaction Options")]
    protected float MaxRange = 5.0f;
    protected float InteractionTime = 1.0f;
    public virtual void UpdateSlot(int slot, string value) {
        infoStrings[slot] = value;
    }
    
    public virtual void UpdateSlot(int slot, int value) {
        infoStrings[slot] = infoStrings[value];
        images[slot] = images[value];
    }
    public virtual void OnStartHover() {
        if (currentLayer != Layers.Selected) {
            SetLayerRecursively(gameObject, (int) Layers.Selection, (int) Layers.Selected);
            currentLayer = Layers.Selected;
            interactionGUI.SetName(displayName);
            if (infoCount > 0) {
                interactionGUI.SetInfo(images, infoStrings, infoCount);
            } else {
                interactionGUI.HideInfo();
            }
            SetInteractionTime(InteractionTime);
        }
    }

    public virtual void SetInteractionTime(float time) {
        interaction.maxIndicatorTimer = time;
    }

    public virtual bool OnInteract() {
        return false;
    }
    
    public virtual bool OnSecondaryInteract() {
        return false;
    }

    public virtual void OnEndHover() {
        if (currentLayer != Layers.Selection) {
            SetLayerRecursively(gameObject, (int) Layers.Selected, (int) Layers.Selection);
            currentLayer = Layers.Selection;
            interactionGUI.HideName();
            interactionGUI.HideInfo();
            SetInteractionTime(0.0f);
        }
    }
}