using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] Actor actor;
    public int id;
    public int dmg = 5;
    [SerializeField] float _velocityY;

    //debug
    private void FixedUpdate()
    {
        _velocityY = actor.rb.velocity.y;
    }


    private void OnCollisionEnter(Collision collision)
    {    
        
        if (_velocityY < -12.0)
        {
            GameEvents.current.HitTriggerEnter(actor, id, 100);
            GameEvents.current.HitTriggerExit(actor, id);
            Debug.Log("hit");
        }

        if (collision.collider.gameObject.CompareTag("Dmg"))
        {           
            GameEvents.current.HitTriggerEnter(actor,id, dmg);
            Debug.Log("hit");
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Dmg"))
        {
            GameEvents.current.HitTriggerExit(actor, id);
        }

    }

}
