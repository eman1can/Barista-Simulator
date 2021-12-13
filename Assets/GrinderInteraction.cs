using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrinderInteraction : IInteractable {
    [SerializeField] private TextMeshPro statusText;
    [SerializeField, Range(1, 5)] private float duration;
    [SerializeField] private GameObject StreamPrefab;
    [SerializeField] private Transform coffeeRoot;
    
    private LiquidDispenser _dispenser;
    private bool _dispensing;
    private LiquidContainer _currentContainer = null;
    private Stream _currentStream = null;
    
    private bool portafilterAttached = false;
    private Portafilter _portafilter;
    private bool hasGrounds = false;
    private bool groundsSpoiled = false;
    private bool isOn = false;
    private bool isGrindingSingle = false;
    private bool isGrindingDouble = false;
    
    public enum Types {
        CURRENT_STATE,
        CURRENT_ACTION,
        CURRENT_SECONDARY_ACTION,
        STATE_OFF,
        STATE_ON,
        STATE_GRINDING_SINGLE_SHOT,
        STATE_GRINDING_DOUBLE_SHOT,
        STATE_PORTAFILTER_MISSING,
        STATE_PORTAFILTER_FULL,
        STATE_PORTAFILTER_SPOILED,
        ACTION_TURN_ON,
        ACTION_WAIT_BUSY,
        ACTION_TURN_OFF,
        ACTION_GRIND_SINGLE_SHOT,
        SECONDARY_ACTION_NONE,
        SECONDARY_ACTION_GRIND_DOUBLE_SHOT
    }
    
    private Types[] slots = {Types.CURRENT_STATE, Types.CURRENT_ACTION, Types.CURRENT_SECONDARY_ACTION};
    private void Awake() {
        _dispenser = GetComponent<LiquidDispenser>();
    }
    
    public override void OnStartHover() {
        UpdateStates();
        PushUpdatedStates();
        base.OnStartHover();
    }

    public void UpdateStates() {
        UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_NONE);
        if (isOn) {
            UpdateSlot(Types.CURRENT_STATE, Types.STATE_ON);
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_OFF);

            if (isGrindingSingle) {
                UpdateSlot(Types.CURRENT_STATE, Types.STATE_GRINDING_SINGLE_SHOT);
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_WAIT_BUSY);
            } else if (isGrindingDouble) {
                UpdateSlot(Types.CURRENT_STATE, Types.STATE_GRINDING_DOUBLE_SHOT);
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_WAIT_BUSY);
            } else {
                if (!portafilterAttached) {
                    UpdateSlot(Types.CURRENT_STATE, Types.STATE_PORTAFILTER_MISSING);
                }else if (hasGrounds) {
                    UpdateSlot(Types.CURRENT_STATE, Types.STATE_PORTAFILTER_FULL);
                } else if (groundsSpoiled) {
                    UpdateSlot(Types.CURRENT_STATE, Types.STATE_PORTAFILTER_SPOILED);
                } else {
                    // If all negative states are not met, then we can brew!
                    UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_GRIND_SINGLE_SHOT);
                    UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_GRIND_DOUBLE_SHOT);
                }
            }
        } else {
            // Machine is off
            UpdateSlot(Types.CURRENT_STATE, Types.STATE_OFF);
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_ON);
        }
    }
    
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
    
    public void PushUpdatedStates() {
        // This is so that we don't have to update every state when we want to update temp or a small var from an action
        interactionGUI.SetInfo(images, infoStrings, infoCount);
    }
    
    public override bool OnInteract() {
        Types action = GetSlot(Types.CURRENT_ACTION);
        switch (action) {
            case Types.ACTION_WAIT_BUSY:
                break;
            case Types.ACTION_TURN_ON:
                TurnOn();
                break;
            case Types.ACTION_TURN_OFF:
                TurnOff();
                break;
            case Types.ACTION_GRIND_SINGLE_SHOT:
                GrindSingleShot();
                break;
        }
        return base.OnInteract();
    }

    public override bool OnSecondaryInteract() {
        Types secondaryAction = GetSlot(Types.CURRENT_SECONDARY_ACTION);
        switch (secondaryAction) {
            case Types.SECONDARY_ACTION_NONE:
                break;
            case Types.SECONDARY_ACTION_GRIND_DOUBLE_SHOT:
                GrindDoubleShot();
                break;
        }
        return base.OnSecondaryInteract();
    }
    
    public void TurnOn() {
        isOn = true;
        UpdateStates();
        PushUpdatedStates();
    }

    public void TurnOff() {
        isOn = false;
        UpdateSlot(Types.CURRENT_STATE, Types.STATE_OFF);
        UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_ON);
        PushUpdatedStates();
    }

    public void GrindSingleShot() {
        isGrindingSingle = true;
        UpdateSlot(Types.CURRENT_STATE, Types.STATE_GRINDING_SINGLE_SHOT);
        UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_WAIT_BUSY);
        PushUpdatedStates();
    }
    
    public void GrindDoubleShot() {
        isGrindingDouble = true;
        UpdateSlot(Types.CURRENT_STATE, Types.STATE_GRINDING_DOUBLE_SHOT);
        UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_WAIT_BUSY);
        PushUpdatedStates();
    }
    

    public void AddPortafilter(HeldObject hObject) {
        portafilterAttached = true;

        _portafilter = hObject.GetComponent<Portafilter>();
        hasGrounds = _portafilter.HasGrounds;
        groundsSpoiled = _portafilter.GroundsSpoiled;
    }
    
    public void RemovePortafilter() {
        portafilterAttached = false;
        _portafilter = null;
    }
    
    private void StartStream() {
        _currentStream = CreateStream();
        _currentContainer = _currentStream.Begin();
        _currentContainer.SetLiquid(_dispenser.GetLiquid());
    }

    private void EndStream() {
        _currentStream.End();
        _currentStream = null;
    }

    private Stream CreateStream() {
        var streamObject = Instantiate(StreamPrefab, coffeeRoot.position, Quaternion.identity, transform);
        return streamObject.GetComponent<Stream>();
    }
}