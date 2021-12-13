using UnityEngine;
using UnityEngine.Events;
/*
 * Created by Ethan Wolfe
 * Will create a new instance of an item and also create a new instance of the material so that they don't interfere
 */
namespace BaristaSimulator
{
    public class SpawnOnInteract : MonoBehaviour, IInteractable, IHoverableEnter
    {
        [SerializeField] private HandManager hand;
        [SerializeField] private GameObject prefab;

        [SerializeField] protected UnityEvent<HeldObject> pickUpEvent;

        [SerializeField] private InformationInterface _info;

        public void OnHoverEnter()
        {
            if (!hand.HandEmpty())
            {
                _info.interactionGUI.HideInfo();
            }
        }

        public void OnInteract()
        {
            if (hand.HandEmpty())
            {
                GameObject newObject = Instantiate(prefab);
                HeldObject heldObject = newObject.GetComponent<HeldObject>();

                LiquidContainer container = newObject.GetComponent<LiquidContainer>();
                GameObject liquidObject = container.GetLiquidObject();
                Renderer renderer = liquidObject.GetComponent<Renderer>();
                Material materialInstance = Instantiate(renderer.material);
                container.UpdateMaterial(renderer, materialInstance);

                hand.PickUpItem(heldObject);
                pickUpEvent.Invoke(heldObject);
            }
        }
    }
}