using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GenericHP 
{
    //fields
    private int _currentHealth;
    private int _currentMaxHealth;

    //properties

    public int Health
    {   
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }
    public int MaxHealth
    {
        get { return _currentMaxHealth; }
        set { _currentMaxHealth = value; }
    }
    //constructor

    public GenericHP(int health , int maxHealth) 
    {
        _currentHealth = health;
        _currentMaxHealth = maxHealth;
    }

    //methods
    public void DmgUnit(int DmgAmount)
    {
        if(_currentHealth >0)
        {
            _currentHealth -= DmgAmount;
        }
    }
    public void HealUnit(int HealAmount)
    {
        if(_currentHealth >0 && _currentHealth < _currentMaxHealth)
        {
            _currentHealth += HealAmount;
        }
        if (_currentHealth > _currentMaxHealth)
        { _currentHealth = _currentMaxHealth;}
    }
}
