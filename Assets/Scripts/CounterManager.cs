using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Created by Ethan Wolfe
 * Works on top of the removeable object system to creating a working space
 */
public class CounterManager : RemoveableObjectManager {
    [SerializeField] private Portafilter portafilter;
    public new enum Types {
        CURRENT_ACTION,
        CURRENT_SECONDARY_ACTION,
        CURRENT_STATE,
        ACTION_REMOVE_OBJECT,
        ACTION_PLACE_OBJECT,
        ACTION_HAND_FULL,
        ACTION_HAND_EMPTY,
        ACTION_HAND_WRONG_OBJECT,
        SECONDARY_ACTION_NONE,
        SECONDARY_ACTION_ADD_GROUNDS,
        SECONDARY_ACTION_TAMP_GROUNDS,
        SECONDARY_ACTION_DUMP_SPOILED_GROUNDS,
        STATE_NO_GROUNDS,
        STATE_GROUNDS_LOOSE,
        STATE_GROUNDS_TAMPED,
        STATE_GROUNDS_SPOILED
        
    }
    private Types[] slots = {Types.CURRENT_ACTION, Types.CURRENT_SECONDARY_ACTION, Types.CURRENT_STATE};
    
    public void UpdateSlot(Types slot, Types value) {
        UpdateSlot((int) slot, (int) value);
        slots[(int) slot] = value;
    }
    
    public void UpdateSlot(Types slot, string value) {
        UpdateSlot((int) slot, value);
    }
    public Types GetSlot(Types slot) {
        return slots[(int) slot];
    }
    private void Awake() {
        objectInSlot = false;
    }

    public override void UpdateStates() {
        infoCount = 1;
        interactionGUI.HideInfo();
        UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_NONE);
        
        if (objectInSlot) {
            if (hand.HandEmpty()) {
                infoCount = 3;
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_REMOVE_OBJECT);
                if (portafilter.HasGrounds) {
                    if (!portafilter.GroundsTamped) {
                        UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_TAMP_GROUNDS);
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_GROUNDS_LOOSE);
                    } else if (portafilter.GroundsSpoiled) {
                        UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_DUMP_SPOILED_GROUNDS);
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_GROUNDS_SPOILED);
                    } else {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_GROUNDS_TAMPED);
                    }
                } else {
                    UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_ADD_GROUNDS);
                    UpdateSlot(Types.CURRENT_STATE, Types.STATE_NO_GROUNDS);
                }
            } else {
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_HAND_FULL);
            }
        } else if (hand.IsHolding(heldObject)) {
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_PLACE_OBJECT);
        } else {
            if (hand.HandEmpty()) {
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_HAND_EMPTY);
            } else {
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_HAND_WRONG_OBJECT);
            }
        }
    }

    public override bool OnInteract() {
        Types action = GetSlot(Types.CURRENT_ACTION);
        
        switch (action) {
            case Types.ACTION_PLACE_OBJECT:
                _heldObject = hand.GetHeldItem();
                addEvent.Invoke(_heldObject);
                hand.PlaceItem(slot);
                objectInSlot = true;
                
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_REMOVE_OBJECT);
                interactionGUI.UpdateStrings(infoStrings, infoCount);
                break;
            case Types.ACTION_REMOVE_OBJECT:
                removeEvent.Invoke();
                hand.PickUpItem(_heldObject);
                _heldObject = null;
                objectInSlot = false;
            
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_PLACE_OBJECT);
                interactionGUI.UpdateStrings(infoStrings, infoCount);
                break;
        }
        
        UpdateStates();
        PushUpdatedStates();
        return false;
    }

    public override bool OnSecondaryInteract() {
        Types secondaryAction = GetSlot(Types.CURRENT_SECONDARY_ACTION);

        switch (secondaryAction) {
            case Types.SECONDARY_ACTION_ADD_GROUNDS:
                portafilter.AddGrounds();
                break;
            case Types.SECONDARY_ACTION_TAMP_GROUNDS:
                portafilter.TampGrounds();
                break;
            case Types.SECONDARY_ACTION_DUMP_SPOILED_GROUNDS:
                portafilter.DumpGrounds();
                break;
        }
        UpdateStates();
        PushUpdatedStates();
        return false;
    }
}