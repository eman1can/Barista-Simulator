using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * The opposite of a liquid container, to be used on things like faucets and coffee makers
 */
namespace BaristaSimulator
{
    public class LiquidDispenser : MonoBehaviour
    {
        [SerializeField] private Liquid liquid;
        // A speed of 1 is equal to 100 ml / s
        public float dispenseSpeed = 1f;
        public float temperature = 16;

        private bool _dispensing = false;
        private float lastDispense = 0f;

        private void Update()
        {
            if (_dispensing)
            {
                lastDispense += Time.deltaTime;
            }
        }
        public void StartDispense()
        {
            _dispensing = true;
        }

        public bool Dispense()
        {
            return _dispensing;
        }
        public void EndDispense()
        {
            _dispensing = false;
        }

        public bool CheckDispense(float time)
        {
            return time <= lastDispense;
        }

        public Liquid GetLiquid()
        {
            return liquid;
        }

        public void Fill(LiquidContainer container)
        {
            lastDispense = 0f;
            container.AddLiquid(Time.deltaTime * dispenseSpeed, temperature);
        }
    }
}