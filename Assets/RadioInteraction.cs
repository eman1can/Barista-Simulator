using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RadioInteraction : IInteractable {
    private bool isOn = true;
    private RadioPlaylist _playlist;

    private void Start() {
        _playlist = GetComponent<RadioPlaylist>();
    }

    public enum Types {
        CURRENT_ACTION,
        CURRENT_SECONDARY_ACTION,
        ACTION_TURN_ON,
        ACTION_TURN_OFF,
        SECONDARY_ACTION_NONE,
        SECONDARY_ACTION_SKIP_SONG
    }
    
    private Types[] slots = {Types.CURRENT_ACTION, Types.CURRENT_SECONDARY_ACTION};
    
    public override void OnStartHover() {
        UpdateStates();
        PushUpdatedStates();
        base.OnStartHover();
    }
    
    public void UpdateStates() {
        if (isOn) {
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_OFF);
            UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_SKIP_SONG);
        } else {
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_ON);
            UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_NONE);
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
            case Types.ACTION_TURN_ON:
                _playlist.ResumeSong();
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_OFF);
                UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_SKIP_SONG);
                break;
            case Types.ACTION_TURN_OFF:
                _playlist.PauseSong();
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_ON);
                UpdateSlot(Types.CURRENT_SECONDARY_ACTION, Types.SECONDARY_ACTION_NONE);
                break;
        }
        PushUpdatedStates();
        return base.OnInteract();
    }

    public override bool OnSecondaryInteract() {
        Types secondaryAction = GetSlot(Types.CURRENT_SECONDARY_ACTION);
        switch (secondaryAction) {
            case Types.SECONDARY_ACTION_NONE:
                break;
            case Types.SECONDARY_ACTION_SKIP_SONG:
                _playlist.SkipSong();
                break;
        }
        return base.OnSecondaryInteract();
    }
}
