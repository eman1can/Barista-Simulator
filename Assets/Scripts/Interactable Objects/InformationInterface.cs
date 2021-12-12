using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Utils;

namespace BaristaSimulator
{
    public class InformationInterface : MonoBehaviour
    {
        [SerializeField] protected RadialInteraction interaction;
        [SerializeField] public InteractionController interactionGUI;
        [SerializeField] protected String displayName;
        [SerializeField] public Sprite[] images;
        [SerializeField] public string[] infoStrings;
        [SerializeField] public int infoCount = 0;

        public Layers currentLayer = Layers.Selection;

        [Header("Interaction Options")]
        protected float MaxRange = 5.0f;
        protected float InteractionTime = 1.0f;
        public void UpdateSlot(int slot, string value)
        {
            infoStrings[slot] = value;
        }

        public void UpdateSlot(int slot, int value)
        {
            infoStrings[slot] = infoStrings[value];
            images[slot] = images[value];
        }

        public void OnStartHover()
        {
            if (currentLayer != Layers.Selected)
            {
                SetLayerRecursively(gameObject, (int)Layers.Selection, (int)Layers.Selected);
                currentLayer = Layers.Selected;
                interactionGUI.SetName(displayName);
                if (infoCount > 0)
                {
                    interactionGUI.SetInfo(images, infoStrings, infoCount);
                }
                else
                {
                    interactionGUI.HideInfo();
                }
                SetInteractionTime(InteractionTime);
            }
        }

        public virtual void SetInteractionTime(float time)
        {
            interaction.maxIndicatorTimer = time;
        }

        public virtual void OnEndHover()
        {
            if (currentLayer != Layers.Selection)
            {
                SetLayerRecursively(gameObject, (int)Layers.Selected, (int)Layers.Selection);
                currentLayer = Layers.Selection;
                interactionGUI.HideName();
                interactionGUI.HideInfo();
                SetInteractionTime(0.0f);
            }
        }
    }
}