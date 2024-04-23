using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



public class GenericPSY 
{
    //fields
    private int _currentPsy;
    private int _currentMaxPsy;
    const float _timeOut = 100;
    float _timer = 100;
    //properties

    public int Psy
    {   
        get { return _currentPsy; }
        set { _currentPsy = value; }
    }
    public int MaxPsy
    {
        get { return _currentMaxPsy; }
        set { _currentMaxPsy = value; }
    }
    //constructor

    public GenericPSY(int psy , int maxPsy) 
    {
        _currentPsy = psy;
        _currentMaxPsy = maxPsy;
    }

    //methods
    public void DecreaseUnit(int amount)
    {       
        if (_currentPsy > 0)
        {
            if (_timer == _timeOut)
            {                
                _currentPsy -= amount;
                _timer = 0;
            }
            else _timer ++;
            
        }
    }
    public void RestoreUnit(int amount)
    {
        if (_currentPsy> 0 && _currentPsy < _currentMaxPsy)
        {
            _currentPsy += amount;
        }
        if (_currentPsy > _currentMaxPsy)
        { _currentPsy = _currentMaxPsy; }
    }
}
