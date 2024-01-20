using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int dmg;

    public bool hit = false;
    
    


    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Dmg"))
        {
            hit = true;
        } else hit = false;
    }
  
   
}
