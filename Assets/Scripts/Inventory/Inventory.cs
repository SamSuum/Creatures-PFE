using PLAYER;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{    
    [SerializeField] Player player;

    [Header("UI")]
    public List<Slot> InventorySlots = new List<Slot>();
    [HideInInspector]public List<Slot> allInventorySlots = new List<Slot>();
    [SerializeField] internal GameObject _inventoryUI;

    [Header("Drag and drop")]
    public Image dragIconImage;

    [SerializeField] internal Item _currentDraggedItem;

    [SerializeField] internal int _currentDragSlotIndex = -1;

    public Transform dropLocation;

    public void ToggleInventory(bool enable, Inventory inv)
    {
        if (!enable)
        {
            foreach (Slot curSlot in inv.InventorySlots)
                curSlot.hovered = false;
        }

        inv._inventoryUI.SetActive(enable);

        Cursor.visible = enable;
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = enable ? 0 : 1;
        
    }


    public  bool CanAddItem(Item item, int amount = 1)
    {
        int freeSpaces = 0;

        foreach (Slot itemSlot in InventorySlots)
        {
            if (itemSlot.getItem() == null)
            {
                return true;
            }
            else if (itemSlot.getItem().ID == item.ID)
            {
                freeSpaces += item.MaximumStacks - itemSlot.slotAmount;
            }
        }
        return freeSpaces >= amount;
    }

    public bool AddItem(Item item, Inventory inv)
    {
        
        for (int i = 0; i < inv.InventorySlots.Count; i++)
        {
            if (inv.CanAddItem(item))
            {
               //stack items
               if (inv.InventorySlots[i].hasItem() && inv.InventorySlots[i].getItem().ID == item.ID)
               {
                    inv.InventorySlots[i].slotAmount += item.Amount;
                    inv.InventorySlots[i].UpdateData();
               }
               else if(inv.InventorySlots[i].getItem() == null)
               {
                    inv.InventorySlots[i].SetItem(item);
                    inv.InventorySlots[i].slotAmount ++;
                    inv.InventorySlots[i].UpdateData();
                }
               else if(inv.InventorySlots[i].hasItem() && inv.InventorySlots[i].getItem().ID != item.ID)
               {
                    continue;
               }

                return true;
            }
        }

        return false;
    }

    public  bool RemoveItem(Item item, Inventory inv)
    {
        for (int i = 0; i < inv.InventorySlots.Count; i++)
        {
            if (inv.InventorySlots[i].getItem() == item)
            {
                if (inv.InventorySlots[i].slotAmount > 0)
                    inv.InventorySlots[i].slotAmount--;
                else
                    inv.InventorySlots[i].SetItem(null); ;
                return true;
            }
        }
        return false;
    }

    internal void RequestUseItem()
    {
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            Slot curSlot = InventorySlots[i];

            if (curSlot.hovered && curSlot.hasItem())
            {
                if(curSlot.getItem().Amount-1 > 0)
                {
                    UseItem(curSlot.getItem());
                    curSlot.slotAmount--;
                    curSlot.UpdateData();
                }
                else
                {
                    curSlot.SetItem(null);
                }
                break;
            }
        }
    }

    internal void UseItem(Item item)
    {
       switch(item.GetItemType())
       {
            case "Healing":
                player.Heal(item.effectAmount);
                break;
            case "Psy":
                player.GainPsy(item.effectAmount);
                break;
       }
    }

    internal void Dropitem()
    {
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            Slot curSlot = InventorySlots[i];

            if (curSlot.hovered && curSlot.hasItem())
            {
                Instantiate(curSlot.getItem().prefab, dropLocation.position, dropLocation.rotation);

                if (curSlot.slotAmount > 0)
                {
                    curSlot.slotAmount--;
                    curSlot.UpdateData();
                }
                else 
                    curSlot.SetItem(null);

                break;
            }
        }
    }
    internal void TransferItem(Item item, Inventory invA, Inventory invB)
    {
        AddItem(item, invB);
        RemoveItem(item, invA);
    }

    internal void DragInventoryIcon()
    {
        for(int i=0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];

            if(curSlot.hovered && curSlot.hasItem())
            {
                _currentDragSlotIndex = i;
                _currentDraggedItem = curSlot.getItem();
                dragIconImage.sprite = _currentDraggedItem.Icon;
                dragIconImage.color = new Color(1, 1, 1, 1);

                curSlot.SetItem(null);
            }
        }
    }

    internal void DropInventoryIcon()
    {
        dragIconImage.sprite = null;
        dragIconImage.color = new Color(1, 1, 1, 0);

        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered)
            {
                if(curSlot.hasItem())
                {
                    Item itemToSwap = curSlot.getItem();

                    curSlot.SetItem(_currentDraggedItem);

                    allInventorySlots[_currentDragSlotIndex].SetItem(itemToSwap);

                    ResetDragVariables();
                    return;
                }
                else
                {
                    curSlot.SetItem(_currentDraggedItem);

                    ResetDragVariables();

                    return;
                }
            }
        }

        allInventorySlots[_currentDragSlotIndex].SetItem(_currentDraggedItem);
        ResetDragVariables();
    }

    internal void ResetDragVariables()
    {
        _currentDraggedItem = null;
        _currentDragSlotIndex = -1;
    }

    
}
