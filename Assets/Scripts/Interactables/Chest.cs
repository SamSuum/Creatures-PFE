using PLAYER;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Inventory, IInteractable
{
    

    [Header("Interaction")]
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;


    [Header("Chest inventory")]
    [SerializeField] private Inventory _playerInv;
    [SerializeField] private List<Slot> _chestSlots = new List<Slot>();

    [Header("Loot tables")]
    [SerializeField] private bool randomLoot;
    [SerializeField] private LootTable lootTable;

    
    private void Start()
    {
        ToggleInventory(false,this);
        InventorySlots.AddRange(_chestSlots);
        allInventorySlots.AddRange(InventorySlots);

        foreach (Slot uiSlot in _chestSlots)
        {
            if (uiSlot != null)
            {
                uiSlot.InitiliazeSlot();
            }
        }

        if(randomLoot)
        {
            lootTable.InitiliazeLootTable();
            lootTable.SpawnLoot(_chestSlots);
        }


    }
    private void Update()
    {
        if (_inventoryUI.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < allInventorySlots.Count; i++)
            {
                Slot curSlot = allInventorySlots[i];

                if (curSlot.hovered && curSlot.hasItem())
                {
                    _playerInv.TransferItem(curSlot.getItem(), this, _playerInv);
                }
            }
           
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ToggleInventory(false,this);
        }
    }


   
    public bool Interact(Actor player)
    {
        ToggleInventory(!_inventoryUI.activeSelf,this);
        return true;
    }

    
   
}
