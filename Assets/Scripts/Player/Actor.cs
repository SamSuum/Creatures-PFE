using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;


/// <summary>
/// Base class for player and npcs controllers
/// </summary>
public abstract class Actor : MonoBehaviour
{
    [Header("CharacterComponents")]
    public Animator _animator;
    public Rigidbody rb;

    public int id;

    internal GenericStamina stamina;
    internal GenericHP HP;
    public StaminaBar staminaBar;
    public PsyBar psyBar;
    public HealthBar healthBar;

    [Header("Animation parameters")]
    public bool grounded;
    public bool sprinting;
    public bool jumping;
    public bool falling;
    public bool attack1;
    public bool attack2;
    public bool blocking;
    public bool dead;   
    public bool combat;
    public bool hit;
    public bool inElevator;

    protected void Init()
    {
        GetAnimator();
        GetRigidbody();
    }      

    #region Animation Handler
    protected void GetAnimator()
    {
        foreach(Transform Child in this.transform)
        {
            if(Child.gameObject.activeSelf == true)
            {
                _animator = GetComponentInChildren<Animator>();
            }
        }
       
    }

    protected bool HasAnimator()
    {
        return (_animator != null) ? true : false;
    }

    protected void UpdateAnimator(float _animationBlend, float motionSpeed)
    {
        _animator.SetFloat("Speed", _animationBlend);
        _animator.SetFloat("MotionSpeed", motionSpeed);
        _animator.SetBool("Jump", jumping);
        _animator.SetBool("Grounded", grounded);
        _animator.SetBool("FreeFall", falling);
        _animator.SetBool("Block", blocking); 
        _animator.SetBool("Attack1", attack1); 
         _animator.SetBool("Attack2", attack2);         
        _animator.SetBool("InCombat",combat);
        _animator.SetBool("Hit", hit);
    }
    #endregion

    #region Statistics Handler
    protected void OnHitTaken(Actor actor,int id,int dmg)
    {
        if(id == actor.id)
        {
            
            actor.HP.DmgUnit(dmg);
            if (actor.stamina != null) actor.stamina.MaxStamina = actor.HP.Health;

            actor.hit = true;
            actor.healthBar.SetHealth(actor.HP.Health);
            
            

            actor.dead = (actor.HP.Health == 0) ? true : false;

            if (actor.dead)
            {
                if (actor.HasAnimator())
                {
                    actor._animator.SetTrigger("Dead");
                    actor._animator.SetLayerWeight(1, 0);
                }
                actor.enabled = false;
                actor.hit = false;
            }
        }
       

    }
    protected void OnHitRecover(Actor actor, int id)
    {
        if(id == actor.id)
        {
            actor.hit = false;
            //implement invincibility frame here
        }

    }
   
    #endregion

    #region Physics
    protected void GetRigidbody()
    {
        rb = GetComponent<Rigidbody>();
    }
    #endregion
}
