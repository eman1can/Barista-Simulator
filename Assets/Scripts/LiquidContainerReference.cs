using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Created by Ethan Wolfe
 * A simple script so that a liquid collider plane can have an easy link to the liquid container it resides in
 */
public class LiquidContainerReference : MonoBehaviour {
    [SerializeField] private LiquidContainer _container;

    public LiquidContainer GetLiquidContianer() {
        return _container;
    }
}