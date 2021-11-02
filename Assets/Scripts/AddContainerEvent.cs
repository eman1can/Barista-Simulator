using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Created by Ethan Wolfe
 * Used for pickup and place event triggers
 */

public class AddContainerEvent : UnityEvent<LiquidContainer> { }

public class RemoveContainerEvent : UnityEvent { };