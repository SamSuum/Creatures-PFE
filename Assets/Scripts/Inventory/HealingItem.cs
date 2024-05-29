using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Items/HealingItem")]
public class HealingItem : Item
{

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif

    public override Item GetCopy()
    {
        return this;
    }

    public override void Destroy()
    {

    }

    public override string GetItemType()
    {
        return "Healing";
    }

    public override string GetDescription()
    {
        return "This item restores "+effectAmount+" Health points.";
    }

    
}
