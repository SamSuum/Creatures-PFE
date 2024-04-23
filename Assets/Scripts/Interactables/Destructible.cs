using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Actor
{
  
    private void Start()
    {
        HP = new GenericHP(100,100);
        stamina = null;
        GameEvents.current.onHitTriggerEnter +=  OnHitTaken;
        GameEvents.current.onHitTriggerExit += OnHitRecover;
    }
    private void OnDestroy()
    {
        GameEvents.current.onHitTriggerEnter -= OnHitTaken;
        GameEvents.current.onHitTriggerExit -= OnHitRecover;
    }
    private void Update()
    {
        if (HasAnimator()) UpdateAnimator(0,0);
    }
}
