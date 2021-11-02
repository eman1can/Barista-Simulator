using UnityEngine;
using UnityEngine.Events;
using static HeldObject;
/*
 * Created by Ethan Wolfe
 * Used to manage a item slot or workspace, such as the portafilter on the espresso machine
 */
public class RemoveableObjectManager : IInteractable {
    [SerializeField] protected HeldObjectType heldObject;
    [SerializeField] protected Transform slot;
    [SerializeField] protected HandManager hand;
    
    [SerializeField] protected UnityEvent<HeldObject> addEvent; 
    [SerializeField] protected UnityEvent removeEvent; 

    [SerializeField] protected bool objectInSlot = true;

    [SerializeField] protected HeldObject _heldObject;

    public enum Types {
        CURRENT_ACTION,
        ACTION_REMOVE_OBJECT,
        ACTION_PLACE_OBJECT,
        ACTION_HAND_FULL,
        ACTION_HAND_EMPTY,
        ACTION_HAND_WRONG_OBJECT
        
    }
    
    public void UpdateSlot(Types slot, Types value) {
        UpdateSlot((int) slot, (int) value);
    }
    
    public void UpdateSlot(Types slot, string value) {
        UpdateSlot((int) slot, value);
    }
    
    public void PushUpdatedStates() {
        interactionGUI.SetInfo(images, infoStrings, infoCount);
    }
    public override void OnStartHover() {
        UpdateStates();
        base.OnStartHover();
    }

    public virtual void UpdateStates() {
        if (objectInSlot) {
            if (hand.HandEmpty()) {
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_REMOVE_OBJECT);
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
        if (objectInSlot && hand.HandEmpty()) {
            removeEvent.Invoke();
            hand.PickUpItem(_heldObject);
            objectInSlot = false;
            _heldObject = null;
            
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_PLACE_OBJECT);
            interactionGUI.UpdateStrings(infoStrings, infoCount);
        } else if (!objectInSlot && hand.IsHolding(heldObject)) {
            _heldObject = hand.GetHeldItem();
            addEvent.Invoke(_heldObject);
            hand.PlaceItem(slot);
            objectInSlot = true;
            
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_REMOVE_OBJECT);
            interactionGUI.UpdateStrings(infoStrings, infoCount);
        }

        return false;
    }
}