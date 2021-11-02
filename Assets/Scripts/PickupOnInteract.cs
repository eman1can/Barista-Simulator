using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using static Utils;
/*
 * Created by Ethan Wolfe
 * Used to transfer an item from a slot to the players hand
 */
public class PickupOnInteract : IInteractable {
    [SerializeField] private HandManager hand;
    
    [SerializeField] protected UnityEvent<HeldObject> pickUpEvent; 
    
    private HeldObject _heldObject;
    
    private void Awake() {
        _heldObject = GetComponent<HeldObject>();
    }

    public override void OnStartHover() {
        base.OnStartHover();
        if (!hand.HandEmpty()) {
            interactionGUI.HideInfo();
        } else {
            UpdateSlot(0, "Pick Up " + hand.GetHeldItemName());
            interactionGUI.UpdateStrings(infoStrings, infoCount);
        }
    }

    public override bool OnInteract() {
        if (hand.HandEmpty()) {
            hand.PickUpItem(_heldObject);
            pickUpEvent.Invoke(_heldObject);
        }
        return false;
    }
}