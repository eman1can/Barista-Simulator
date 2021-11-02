using UnityEngine;
using static HeldObject;
/*
 * Created by Ethan Wolfe
 * Used for pickup and place event triggers
 */
public class HandManager : MonoBehaviour {
    [SerializeField] private Transform handTransform;
    
    private HeldObject item = null;

    public bool IsHolding(HeldObjectType itemType) {
        if (this.item == null)
            return false;
        if (itemType == null)
            return false;
        return this.item.GetItemType() == itemType;
    }

    public bool HandEmpty() {
        return item == null;
    }

    public void PickUpItem(HeldObject newItem) {
        if (HandEmpty()) {
            item = newItem;
            Transform itemTransform = item.gameObject.transform;
            itemTransform.SetParent(handTransform);
            
            Transform centerPivot = item.GetCenterPivot();
            itemTransform.localPosition = centerPivot.localPosition;
            itemTransform.localRotation = centerPivot.localRotation;
        }
    }

    public void PlaceItem(Transform placeTransform) {
        if (!HandEmpty()) {
            Transform itemTransform = item.gameObject.transform;
            itemTransform.SetParent(placeTransform);
            
            itemTransform.localPosition = new Vector3(0, 0, 0);
            itemTransform.localRotation = Quaternion.Euler(0, 0, 0);
            
            item = null;
        }
    }

    public string GetHeldItemName() {
        if (item == null)
            return "";
        return item.GetName();
    }

    public HeldObject GetHeldItem() {
        return item;
    }
}