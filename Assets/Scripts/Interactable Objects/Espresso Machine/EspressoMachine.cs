using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Created by Ethan Wolfe
 * An extensive class to control the operation of the espresso machine and all of its states
 */
namespace BaristaSimulator
{
    public class EspressoMachine: MonoBehaviour, IHoverableEnter, IInteractable
    {
        public new float MaxRange => 5;
        public new float InteractionTime => 1;

        [SerializeField] private WeatherController _weatherController;
        [SerializeField] private TextMeshPro temperatureDisplay;
        [SerializeField] private Liquid coffee;

        private bool portafilterAttached = true;
        private bool waterContainerAttached = true;
        private bool dripTrayAttached = true;
        private bool cupAttached = false;

        // Numeric state variables
        private float waterLevel = .0f;
        private float dripTrayLevel = .0f;
        private float dripTrayMax = 5.0f;
        private float cupLevel = .0f;
        private float cupMax = .2f;
        private float temperature = .0f;
        private float brewTime = .0f;

        // State booleans
        private bool hasGrounds = false;
        private bool groundsTamped = false;
        private bool groundsSpoiled = false;
        private bool isOn = false;
        private bool isBrewing = false;
        private bool heating = false;

        // Grab the containers / portafilter when added
        private WaterContainer _waterContainer;
        private PortableFilter _portafilter;
        private DripTray _dripTray;
        private CoffeeCup _cup;

        [SerializeField] private InformationInterface _info;

        public enum Types
        {
            CURRENT_STATE,
            CURRENT_TEMPERATURE,
            CURRENT_ACTION,
            STATE_OFF,
            STATE_ON,
            STATE_PORTAFILTER_MISSING,
            STATE_WATER_CONTAINER_MISSING,
            STATE_DRIP_TRAY_MISSING,
            STATE_CUP_MISSING,
            STATE_NO_GROUNDS,
            STATE_GROUNDS_LOOSE,
            STATE_GROUNDS_SPOILED,
            STATE_NO_WATER,
            STATE_DRIP_TRAY_FULL,
            STATE_CUP_FULL,
            STATE_LOW_TEMPERATURE,
            STATE_BREWING,
            ACTION_TURN_ON,
            ACTION_TURN_OFF,
            ACTION_START_BREWING,
            ACTION_STOP_BREWING
        }

        private Types[] slots = { Types.CURRENT_STATE, Types.CURRENT_TEMPERATURE, Types.CURRENT_ACTION };
        public void RemoveWaterContainer()
        {
            waterContainerAttached = false;
            _waterContainer = null;
        }

        public void RemovePortafilter()
        {
            portafilterAttached = false;
            _portafilter = null;
        }

        public void RemoveDripTray()
        {
            dripTrayAttached = false;
            _dripTray = null;
        }

        public void RemoveCup()
        {
            cupAttached = false;
            _cup = null;
        }

        public void AddWaterContainer(HeldObject hObject)
        {
            waterContainerAttached = true;

            _waterContainer = hObject.GetComponent<WaterContainer>();
            waterLevel = _waterContainer.GetFillAmount();
        }

        public void AddPortafilter(HeldObject hObject)
        {
            portafilterAttached = true;

            _portafilter = hObject.GetComponent<PortableFilter>();
            hasGrounds = _portafilter.HasGrounds;
            groundsTamped = _portafilter.GroundsTamped;
            groundsSpoiled = _portafilter.GroundsSpoiled;
        }

        public void AddDripTray(HeldObject hObject)
        {
            dripTrayAttached = true;

            _dripTray = hObject.GetComponent<DripTray>();
            dripTrayLevel = _dripTray.GetFillAmount();
            dripTrayMax = _dripTray.GetFillMax();
        }

        public void AddCup(HeldObject hObject)
        {
            cupAttached = true;

            _cup = hObject.GetComponent<CoffeeCup>();
            cupLevel = _cup.GetFillAmount();
            cupMax = _cup.GetFillMax();
        }
        public void UpdateSlot(Types slot, Types value)
        {
            _info.UpdateSlot((int)slot, (int)value);
            slots[(int)slot] = value;
        }
        public void UpdateSlot(Types slot, string value)
        {
            _info.UpdateSlot((int)slot, value);
        }

        public Types GetSlot(Types slot)
        {
            return slots[(int)slot];
        }
        public void UpdateStates()
        {
            UpdateSlot(Types.CURRENT_TEMPERATURE, ((int)temperature).ToString());
            temperatureDisplay.text = ((int)temperature) + "°C";

            if (isOn)
            {
                UpdateSlot(Types.CURRENT_STATE, Types.STATE_ON);
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_OFF);

                // Is the machine brewing a coffee?
                if (isBrewing)
                {
                    UpdateSlot(Types.CURRENT_STATE, Types.STATE_BREWING);
                    UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_STOP_BREWING);
                }
                else
                {
                    // Negative states that stop brewing
                    if (!portafilterAttached)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_PORTAFILTER_MISSING);
                    }
                    else if (!waterContainerAttached)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_WATER_CONTAINER_MISSING);
                    }
                    else if (!dripTrayAttached)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_DRIP_TRAY_MISSING);
                    }
                    else if (!cupAttached)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_CUP_MISSING);
                    }
                    else if (!hasGrounds)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_NO_GROUNDS);
                    }
                    else if (!groundsTamped)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_GROUNDS_LOOSE);
                    }
                    else if (groundsSpoiled)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_GROUNDS_SPOILED);
                    }
                    else if (waterLevel < 0.20f)
                    {
                        // Water level is 1 -> 10; We will say 0.2 per coffee
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_NO_WATER);
                    }
                    else if (dripTrayLevel > dripTrayMax * 0.9f)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_DRIP_TRAY_FULL);
                    }
                    else if (cupLevel > cupMax * 0.9f)
                    {
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_CUP_FULL);
                    }
                    else if (temperature < 88)
                    {
                        // Temperature is in celcius
                        UpdateSlot(Types.CURRENT_STATE, Types.STATE_LOW_TEMPERATURE);
                    }
                    else
                    {
                        // If all negative states are not met, then we can brew!
                        UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_START_BREWING);
                    }
                }
            }
            else
            {
                // Machine is off
                UpdateSlot(Types.CURRENT_STATE, Types.STATE_OFF);
                UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_ON);
            }
        }

        public void PushUpdatedStates()
        {
            // This is so that we don't have to update every state when we want to update temp or a small var from an action
            _info.interactionGUI.SetInfo(_info.images, _info.infoStrings, _info.infoCount);
        }

        private void Awake()
        {
            temperature = _weatherController.GetAmbientTemperature();
            UpdateSlot(Types.CURRENT_TEMPERATURE, ((int)temperature).ToString());
            temperatureDisplay.text = ((int)temperature) + "°C";
        }

        public void OnHoverEnter()
        {
            UpdateStates();
            PushUpdatedStates();
        }

        public void OnInteract()
        {
            Types action = GetSlot(Types.CURRENT_ACTION);
            switch (action)
            {
                case Types.ACTION_TURN_ON:
                    TurnOn();
                    break;
                case Types.ACTION_TURN_OFF:
                    TurnOff();
                    break;
                case Types.ACTION_START_BREWING:
                    StartBrewing();
                    break;
                case Types.ACTION_STOP_BREWING:
                    StopBrewing();
                    break;
            }
        }

        public void TurnOn()
        {
            isOn = true;
            if (temperature < 93.33f)
                heating = true;
            UpdateStates();
            StartBrewing();
            PushUpdatedStates();
        }

        public void TurnOff()
        {
            isOn = false;
            heating = false;
            temperatureDisplay.text = "-- °C";
            UpdateSlot(Types.CURRENT_STATE, Types.STATE_OFF);
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_TURN_ON);
            PushUpdatedStates();
        }

        public void StartBrewing()
        {
            isBrewing = true;
            UpdateSlot(Types.CURRENT_STATE, Types.STATE_BREWING);
            UpdateSlot(Types.CURRENT_ACTION, Types.ACTION_STOP_BREWING);

            // TODO: Start brew stream
            brewTime = .0f;
            PushUpdatedStates();
        }

        public void StopBrewing()
        {
            isBrewing = false;
            groundsSpoiled = true;
            if (_portafilter != null)
                _portafilter.SpoilGrounds();
            UpdateStates();
            PushUpdatedStates();
            // TODO: Stop brew stream
            _cup.SetLiquid(coffee);
            _cup.AddLiquid(0.2f, 20f);
        }

        private void Update()
        {
            // Update temperature
            float delta;
            if (heating)
            {
                delta = 0.75f * Time.deltaTime;
            }
            else
            {
                if (isOn)
                {
                    delta = -0.25f * Time.deltaTime;
                }
                else if (temperature > _weatherController.GetAmbientTemperature())
                {
                    delta = -1.5f * Time.deltaTime;
                }
                else
                {
                    delta = 0f;
                }
            }

            if (temperature < 88f && temperature + delta > 88f)
            {
                UpdateStates();
            }

            temperature += delta;

            // Update brewing states
            if (isOn && isBrewing)
            {
                // TODO: Update brew stream
                brewTime += Time.deltaTime;
                if (brewTime > 2f)
                {
                    StopBrewing();
                }
            }

            // push temperature update and calculate heating
            UpdateSlot(Types.CURRENT_TEMPERATURE, ((int)temperature).ToString());
            if (isOn)
                temperatureDisplay.text = ((int)temperature) + "°C";

            if (heating && temperature > 95f)
                heating = false;
            if (isOn && !heating && temperature < 88f)
                heating = true;

            if (_info.currentLayer == Utils.Layers.Selected)
            {
                PushUpdatedStates();
            }
        }
    }
}