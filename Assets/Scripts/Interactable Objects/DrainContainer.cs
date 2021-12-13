using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Modifies a liquid container to drain over time
 */
namespace BaristaSimulator
{
    public class DrainContainer : LiquidContainer
    {
        // Drain speed is in ml/s
        [SerializeField] private float drainSpeed;
        void Update()
        {
            if (_amount > 0)
            {
                RemoveLiquid(drainSpeed * Time.deltaTime);
            }
        }
    }
}