using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Item : MonoBehaviour, IInteractable
{
    public new string name = "new item";
    public Sprite icon;
    public string description = "...";
    public int currentQuantity = 1;
    public int maxQuantitity = 10;
    [SerializeField] private string _interactionPrompt = "Take";

    [Header("item use")]
    public UnityEvent myEvent;
    public bool removeOneOnUse;


    public string InteractionPrompt => _interactionPrompt;

    public bool Interact(Actor player)
    {
        player.gameObject.GetComponent<Inventory>().AddItemToInventory(this);

        return true;
    }

   
    public void UseItem()
    {
        if(myEvent.GetPersistentEventCount()>0)
        {
            myEvent.Invoke();

            if (removeOneOnUse)
                currentQuantity--;
        }
    }
}
