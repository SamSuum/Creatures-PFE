using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GenericStamina 
{
    //fields
    private int _currentStamina;
    private int _currentMaxStamina;
    const int _timeOut = 10;
    int _timer = 10;

    //properties

    public int Stamina
    {   
        get { return _currentStamina; }
        set { _currentStamina = value; }
    }
    public int MaxStamina
    {
        get { return _currentMaxStamina; }
        set { _currentMaxStamina = value; }
    }
    //constructor

    public GenericStamina(int stamina , int maxStamina) 
    {
        _currentStamina = stamina;
        _currentMaxStamina = maxStamina;
    }

    //methods
    public void DecreaseUnit(int amount)
    {
        if(_currentStamina > 0)
        {
            if (_timer == _timeOut)
            {
                _currentStamina -= amount;
                _timer = 0;
            }
            else _timer++;
        }
       
    }
    public void RestoreUnit(int amount)
    {
        if(_currentStamina >= 0 && _currentStamina < _currentMaxStamina)
        {
            if (_timer == _timeOut)
            { 
                _currentStamina += amount;
                _timer = 0;
            }
            else _timer++;
    }
        if (_currentStamina > _currentMaxStamina)
        { _currentStamina = _currentMaxStamina; }
       
    }
}
