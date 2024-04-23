using PLAYER;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string _prompt;
    public string InteractionPrompt => _prompt;


    [Header("Chest inventory")]
    [SerializeField] private Inventory _playerInv;
    [SerializeField] private List<Slot> _chestSlots = new List<Slot>();
    [SerializeField] private GameObject _inventoryUI;

    [Header("Loot tables")]
    [SerializeField] private bool randomLoot;
    [SerializeField] private LootTable lootTable;

    Player _player;
    private void Awake()
    {
        _player = _playerInv.GetComponent<Player>();
    }
    private void Start()
    {
        ToggleChest(false);

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
        if (_inventoryUI.activeInHierarchy && UnityEngine.Input.GetMouseButtonDown(0))
        {
            TakeItem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ToggleChest(false);
        }
    }


    private void TakeItem()
    {       

        for (int i = 0; i < _chestSlots.Count; i++)
        {
            Slot curSlot = _chestSlots[i];

            if (curSlot.hovered && curSlot.hasItem())
            {                
                _playerInv.allInventorySlots[i].SetItem(curSlot.getItem());
                _playerInv.allInventorySlots[i].getItem().currentQuantity = curSlot.getItem().currentQuantity;
                _playerInv.allInventorySlots[i].UpdateData();

                curSlot.SetItem(null);               
                curSlot.UpdateData();

                break;
            }
        }
    }

    public bool Interact(Actor player)
    {
        ToggleChest(!_inventoryUI.activeSelf);
        return true;
    }

    private void ToggleChest(bool enable)
    {
        if (!enable)
        {
            foreach (Slot curSlot in _chestSlots)
                curSlot.hovered = false;
        }

        _inventoryUI.SetActive(enable);

        Cursor.visible = enable;
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
    }

   
}
