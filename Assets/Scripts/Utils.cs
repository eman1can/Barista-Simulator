using UnityEngine;
/*
 * Created by Ethan Wolfe
 * Code sourced from https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
 * Used to set the selection layer recursively
 * 
 */
public class Utils {
    public enum Layers { Default, TransparentFx, IgnoreRaycast, Empty, Water, UI, Selection, Selected, PostProcessing, NotInReflection, LiquidCollision }
    public static void SetLayerRecursively(GameObject obj, int maskLayer, int layer) {
        if (obj.layer == maskLayer) {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, maskLayer, layer);
        }
    }
}