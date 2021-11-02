using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Opposite of pickup, will palce an item in a slot
 */
public class PlaceOnInteract : IInteractable {
    [SerializeField] private Transform pivot;
    
    [SerializeField] private HandManager hand;

    public override void OnStartHover() {
        base.OnStartHover();
        if (hand.HandEmpty()) {
            interactionGUI.HideInfo();
        } else {
            UpdateSlot(0, "Place " + hand.GetHeldItemName());
            interactionGUI.UpdateStrings(infoStrings, infoCount);
        }
    }

    public override bool OnInteract() {
        if (!hand.HandEmpty()) {
            hand.PlaceItem(pivot);
            interactionGUI.HideInfo();
        }

        return false;
    }
}