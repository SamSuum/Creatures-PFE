using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovered;
    [SerializeField] private Item _heldItem;
    private Color _opaque = new Color(1, 1, 1, 1);
    private Color _transparent = new Color(1, 1, 1, 0);

    [SerializeField] private Image _thisSlotImage;
    public TMP_Text thisSlotQuantityText;

    [SerializeField] private GameObject _descriptionPanel;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _name;


    public int slotAmount;

    public void InitiliazeSlot()
    {
        _thisSlotImage = gameObject.GetComponent<Image>();
        thisSlotQuantityText = transform.GetChild(1).GetComponent<TMP_Text>();

        _descriptionPanel.SetActive(false);

        _thisSlotImage.sprite = null;
        _thisSlotImage.color = _transparent;
        SetItem(null);
    }

    public void SetItem(Item item)
    {
        _heldItem = item;

        if(item!=null)
        {
            _thisSlotImage.sprite = _heldItem.Icon;
            slotAmount = _heldItem.Amount;
            _thisSlotImage.color = _opaque;

            _name.text = _heldItem.ItemName;
            _description.text = _heldItem.GetDescription();

            UpdateData();
        }
        else
        {
            slotAmount = 0;
            _thisSlotImage.sprite = null;
            _thisSlotImage.color = _transparent;
            _name.text = "";
            _description.text = "";
            UpdateData();
        }
    }
    public void UpdateData()
    {
        if(_heldItem != null)
        {
            thisSlotQuantityText.text =slotAmount.ToString();
        }
        else
        {
            thisSlotQuantityText.text = "";
        }
    }
    public Item getItem()
    {
        return _heldItem;
    }

    public bool hasItem()
    {
        return _heldItem ? true : false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        if (hasItem()) _descriptionPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        _descriptionPanel.SetActive(false);
    }

   
}