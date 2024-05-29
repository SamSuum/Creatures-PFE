using PLAYER;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Inventory, IInteractable
{
    [SerializeField] Animator anim;

    [Header("Interaction")]
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;


    [Header("Chest inventory")]
    [SerializeField] private Inventory _playerInv;
    [SerializeField] private List<Slot> _chestSlots = new List<Slot>();
    [SerializeField] private List<Item> items = new List<Item>();

    [Header("Loot tables")]
    [SerializeField] private bool randomLoot;
    [SerializeField] private LootTable lootTable;

    
    private void Start()
    {     

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
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                _chestSlots[i].SetItem(items[i]);                
            }
        }

        ToggleInventory(false, this);
        InventorySlots.AddRange(_chestSlots);
        allInventorySlots.AddRange(InventorySlots);
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
        Time.timeScale = 1;
        anim.SetBool("Open", _inventoryUI.activeSelf);

        return true;
    }

    
   
}
