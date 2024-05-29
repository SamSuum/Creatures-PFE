using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] Actor actor;
    public int id;
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
        }

        if (collision.collider.gameObject.CompareTag("Dmg") && collision.collider.gameObject.GetComponent<WeaponID>().id != id)
        {           
            GameEvents.current.HitTriggerEnter(actor,id, collision.collider.gameObject.GetComponent<WeaponID>().dmg);
            GameEvents.current.HitTriggerExit(actor, id);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dmg") && other.gameObject.GetComponent<WeaponID>().id != id)
        {
            GameEvents.current.HitTriggerEnter(actor, id, other.gameObject.GetComponent<WeaponID>().dmg);
            GameEvents.current.HitTriggerExit(actor, id);
        }

    }

}
