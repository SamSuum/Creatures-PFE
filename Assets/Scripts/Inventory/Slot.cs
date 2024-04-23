using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovered;
    [SerializeField] private Item _heldItem;
    private Color _opaque = new Color(1, 1, 1, 1);
    private Color _transparent = new Color(1, 1, 1, 0);

    [SerializeField] private Image _thisSlotImage;
    public TMP_Text thisSlotQuantityText;

    public void InitiliazeSlot()
    {
        _thisSlotImage = gameObject.GetComponent<Image>();
        thisSlotQuantityText = transform.GetChild(0).GetComponent<TMP_Text>();
        _thisSlotImage.sprite = null;
        _thisSlotImage.color = _transparent;
        SetItem(null);
    }

    public void SetItem(Item item)
    {
        _heldItem = item;

        if(item!=null)
        {
            _thisSlotImage.sprite = _heldItem.icon;
            _thisSlotImage.color = _opaque;
            UpdateData();
        }
        else
        {
            _thisSlotImage.sprite = null;
            _thisSlotImage.color = _transparent;
            UpdateData();
        }
    }
    public void UpdateData()
    {
        if(_heldItem != null)
        {
            thisSlotQuantityText.text = _heldItem.currentQuantity.ToString();
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}