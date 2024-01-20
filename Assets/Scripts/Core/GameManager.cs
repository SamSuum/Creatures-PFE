using Ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

   public static GameManager gameManager
    {
        get; private set;
    }


    public GenericHP _playerHealth = new GenericHP(100, 100);

    private void Awake()
    {
        if (gameManager != null && gameManager != this) 
        {
            Destroy(this);
        }
        else  gameManager = this;
    }

}