using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ItemStack : MonoBehaviour, IInteractable
{
    public Item item;
    [SerializeField] int amount = 1;

    [SerializeField] private string _interactionPrompt = "Take";

    public string InteractionPrompt => _interactionPrompt;

    public bool Interact(Actor player)
    {
        Inventory playerInv = player.gameObject.GetComponent<Inventory>();

        Item itemCopy = item.GetCopy();

        if(playerInv.AddItem(itemCopy,playerInv))
        {
            amount--;

            if (amount == 0)
                Destroy(this.gameObject);
        }
        else
        {
            itemCopy.Destroy();
        }       

        return true;
    }

   
    
}
