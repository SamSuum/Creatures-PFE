using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] PlayerInputs input;

    [Header("UI")]
    [HideInInspector] public List<Slot> allInventorySlots = new List<Slot>();
    public List<Slot> inventorySlots = new List<Slot>();

    public Transform dropLocation;

    [Header("Drag and drop")]
    public Image dragIconImage;

    [SerializeField] private Item _currentDraggedItem;

    [SerializeField] private int _currentDragSlotIndex = -1;



    private void Start()
    {
        allInventorySlots.AddRange(inventorySlots);

        ToggleInventory(false);

        foreach (Slot uiSlot in allInventorySlots)
        {
            if (uiSlot != null)
            {
                uiSlot.InitiliazeSlot();
            }            
        }


    }
    private void Update()
    {
        if (input.menu)
        {
            ToggleInventory(!_inventoryUI.activeSelf);
        }

        if (_inventoryUI.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            DragInventoryIcon();
        }
        else if(_currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0)  || _currentDragSlotIndex!=-1 && !_inventoryUI.activeInHierarchy)
        {
            DropInventoryIcon();
        }
        if (Input.GetKeyDown(KeyCode.Q))
            Dropitem();

        dragIconImage.transform.position = Input.mousePosition;

        if(Input.GetMouseButtonDown(1))
        {
            RequestUseItem();
        }
    }

    public void ToggleInventory(bool enable)
    {
        if (!enable)
        {
            foreach (Slot curSlot in allInventorySlots)
                curSlot.hovered = false;
        }

        _inventoryUI.SetActive(enable);

        Cursor.visible = enable;
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = enable ? 0 : 1;
        
    }

    public void AddItemToInventory(Item item)
    {
        int leftoverQuantity = item.currentQuantity;
        Slot openSlot = null;

        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Item heldItem;

            if (allInventorySlots[i] != null)
            {
                heldItem = allInventorySlots[i].getItem();
            }

            else
            {
                heldItem = null;
                Debug.Log("Inventory Slot null");
            }

            if (heldItem != null && item.name == heldItem.name)
            {
                int freeSpaceInSLot = heldItem.maxQuantitity - heldItem.currentQuantity;
                if (freeSpaceInSLot >= leftoverQuantity)
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(item.gameObject);
                    allInventorySlots[i].UpdateData();
                    return;
                }
                else
                {
                    heldItem.currentQuantity = heldItem.maxQuantitity;
                    leftoverQuantity -= freeSpaceInSLot;
                }

            }
            else if (heldItem == null)
            {
                if (!openSlot)
                    openSlot = allInventorySlots[i];
            }

            allInventorySlots[i].UpdateData();
        }

        if (leftoverQuantity > 0 && openSlot)
        {
            openSlot.SetItem(item);
            item.currentQuantity = leftoverQuantity;
            item.gameObject.SetActive(false);
        }
        else
        {
            item.currentQuantity = leftoverQuantity;
        }
    }

    private void RequestUseItem()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];

            if (curSlot.hovered && curSlot.hasItem())
            {
                curSlot.getItem().UseItem();

                if(curSlot.getItem().currentQuantity !=0)
                {
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

    private void Dropitem()
    {
       for(int i= 0; i< allInventorySlots.Count;i++)
        {
            Slot curSlot = allInventorySlots[i];

            if(curSlot.hovered && curSlot.hasItem())
            {
                curSlot.getItem().gameObject.SetActive(true);
                curSlot.getItem().transform.position = dropLocation.position;
                curSlot.SetItem(null);
                break;
            }
        }
    }

    private void DragInventoryIcon()
    {
        for(int i=0; i< allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];

            if(curSlot.hovered && curSlot.hasItem())
            {
                _currentDragSlotIndex = i;
                _currentDraggedItem = curSlot.getItem();
                dragIconImage.sprite = _currentDraggedItem.icon;
                dragIconImage.color = new Color(1, 1, 1, 1);

                curSlot.SetItem(null);
            }
        }
    }

    private void DropInventoryIcon()
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

    private void ResetDragVariables()
    {
        _currentDraggedItem = null;
        _currentDragSlotIndex = -1;
    }

    
}
