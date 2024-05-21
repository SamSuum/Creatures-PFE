using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Inventory
{
    [SerializeField] PlayerInputs input;
    private void Start()
    {
        ToggleInventory(false,this);

        foreach (Slot uiSlot in InventorySlots)
        {
            if (uiSlot != null)
            {
                uiSlot.InitiliazeSlot();
            }
        }

        allInventorySlots.AddRange(InventorySlots);
    }
    private void Update()
    {
        if (input.menu)
        {
            ToggleInventory(!_inventoryUI.activeSelf,this);
        }
        
        if (_inventoryUI.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            DragInventoryIcon();
        }
        else if (_currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0) || _currentDragSlotIndex != -1 && !_inventoryUI.activeInHierarchy)
        {
            DropInventoryIcon();
        }   
       
        dragIconImage.transform.position = Input.mousePosition;
        

        if (Input.GetKeyDown(KeyCode.Q))
            Dropitem();

        if (Input.GetMouseButtonDown(1))
        {
            RequestUseItem();
        }
    }

   



}
