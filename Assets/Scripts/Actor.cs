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
    //animation parameters
    [HideInInspector] public bool grounded;
    [HideInInspector] public bool sprinting;
    [HideInInspector] public bool jumping;
    [HideInInspector] public bool falling;
    [HideInInspector] public bool attack1;
    [HideInInspector] public bool attack2;
    [HideInInspector] public bool blocking;
    [HideInInspector] public bool dead;
    [HideInInspector] public bool combat;
    public bool hit;
    [HideInInspector] public bool inElevator;


    public float damageCooldown = 0;
    public float damageDelay = 1.0f;

    public virtual void OnStart() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnUpdate() { }
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
        if(damageCooldown > 0)
        {
            return;
        }
        else if(id == actor.id && actor.blocking == false)
        {
            actor.healthBar.gameObject.SetActive(true);

            actor.HP.DmgUnit(dmg);

            if (actor.stamina != null) actor.stamina.MaxStamina = actor.HP.Health;           

            if(actor.healthBar!= null) actor.healthBar.SetHealth(actor.HP.Health);

            actor.dead = (actor.HP.Health == 0) ? true : false;            

            actor.damageCooldown = actor.damageDelay;
        }
    }
    protected void OnDeath(Actor actor)
    {        
        if (actor.HasAnimator())
        {
            actor._animator.SetTrigger("Dead");
            actor._animator.SetLayerWeight(1, 0);
        }

        actor.healthBar.gameObject.SetActive(false);
        actor.hit = false;
        actor.enabled = false;
    }


    protected void OnHitRecover(Actor actor, int id)
    {
        if (id == actor.id)
        {           
            StartCoroutine(Wait(damageDelay,actor));           
        }
    }

    private IEnumerator Wait(float amount,Actor actor)
    {
        actor.hit = true;

        Debug.Log("Waiting");
        yield return new WaitForSeconds(amount);

        actor.hit = false;
    }
    protected void UpdateDamageCoolDown(Actor actor)
    {
        actor.damageCooldown -= Time.deltaTime;

        if (actor.damageCooldown < 0)
            actor.damageCooldown = 0;
    }

    

    #endregion

    #region Physics
    protected void GetRigidbody()
    {
        rb = GetComponent<Rigidbody>();
    }
    #endregion
}
