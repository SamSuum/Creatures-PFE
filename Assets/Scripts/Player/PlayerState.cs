using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.FilePathAttribute;


public abstract class PlayerBaseState 
{
    public string name;
    protected PlayerStateMachine _stateMachine;
    protected Player sm;
    public PlayerBaseState(string name, PlayerStateMachine stateMachine)
    {
        this.name = name;
        this._stateMachine = stateMachine;
    }
    public virtual void Enter(){}
    public virtual void UpdateLogic(){}
    public virtual void UpdatePhysics() {}
    public virtual void Exit(){}
    
}


public class InAir : PlayerBaseState
{
  
    public InAir(Player stateMachine) : base("Falling", stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        sm.falling = true;
        sm.jumping = false;
    }
    public override void UpdatePhysics()
    {
        
        if (sm.grounded) 
        {
            sm.ChangeState(sm.idleState);
        }
    }
    public override void UpdateLogic()
    {
        Fall();
    }

    public override void Exit()
    {
        sm.falling = false;
    }

    private void Fall()
    {
        // reset the jump timeout timer
        sm.jumpTimeoutDelta = sm.JumpTimeout;

        // fall timeout
        if (sm.fallTimeoutDelta >= 0.0f)
        {
            sm.fallTimeoutDelta -= Time.deltaTime;
        }

        // if we are not grounded, do not jump
        sm.input.jump = false;

    }

}


public class Grounded : PlayerBaseState
{
   

    public Grounded(string name, Player stateMachine) : base(name, stateMachine){
        sm = stateMachine;
    }
    public override void Enter()
    {
        sm.falling = false;
        sm.jumping = false;
    }
    public override void UpdatePhysics()
    {
        if (!sm.grounded) sm.ChangeState(sm.inAirState);        

        HandleVerticalVelocity();
        HandleHorizontalVelocity();
        HandleTargetSpeed();

        if (sm.input.jump && sm.jumpTimeoutDelta <= 0.0f && sm.stamina.Stamina > 0)
        {
            Jump();
        }
        else
        {
            sm.input.jump = false;
            sm.jumping = false;
        }

        if(OnSlope())
        {
            MoveOnSlope();
        }

        HandleStepClimb();
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

    //Velocity
    private void HandleTargetSpeed()
    {
        if (sm.input.move != Vector2.zero)
        {
            if (sm.input.sprint && sm.stamina.Stamina > 0)
            {
                sm.targetSpeed = sm.SprintSpeed;

                sm.stamina.DecreaseUnit(1);
                sm.staminaBar.SetStamina(sm.stamina.Stamina);

                sm.sprinting = true;
            }
            else
            {
                sm.targetSpeed = sm.MoveSpeed;
                sm.sprinting = false;
            }
        }
        else
        {
            sm.targetSpeed = 0.0f;
            sm.sprinting = false;

            sm.stamina.RestoreUnit(1);
            sm.staminaBar.SetStamina(sm.stamina.Stamina);
        }
    }
    public void HandleVerticalVelocity()
    {
        // reset the fall timeout timer
        sm.fallTimeoutDelta = sm.FallTimeout;

        // stop our velocity dropping infinitely when grounded
        if (sm.verticalVelocity < 0.0f)
        {
            sm.verticalVelocity = -2f;
        }

        // jump timeout
        if (sm.jumpTimeoutDelta >= 0.0f)
        {
            sm.jumpTimeoutDelta -= Time.deltaTime;
        }

        if (sm.verticalVelocity < sm.terminalVelocity)
        {
            sm.verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }
    public void HandleHorizontalVelocity()
    {

        float currentHorizontalSpeed = new Vector3(sm.rb.velocity.x, 0.0f, sm.rb.velocity.z).magnitude;
        float speedOffset = 0.1f;
        sm.Inputmagnitude = sm.input.analogMovement ? sm.input.move.magnitude : 1f;


        if (currentHorizontalSpeed < sm.targetSpeed - speedOffset || currentHorizontalSpeed > sm.targetSpeed + speedOffset)
        {
            sm.speed = Mathf.Lerp(currentHorizontalSpeed, sm.targetSpeed * sm.Inputmagnitude, Time.deltaTime * sm.SpeedChangeRate);
            // round speed to 3 decimal places
            sm.speed = Mathf.Round(sm.speed * 1000f) / 1000f;
        }
        else sm.speed = sm.targetSpeed;
    }

    private void Jump()
    {
        sm.staminaBar.SetStamina(sm.stamina.Stamina);
        sm.stamina.DecreaseUnit(5);

        sm.jumping = true;
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        sm.verticalVelocity = Mathf.Sqrt(sm.JumpHeight * -2f * Physics.gravity.y);
        sm.rb.AddForce(Vector3.up * sm.verticalVelocity, ForceMode.Impulse);
    }


    RaycastHit slopeHit;    
    float playerHeight = 2.0f;
    private bool OnSlope()
    {
        //replace raycast with checkcapsule
        if (Physics.Raycast(sm.transform.position,Vector3.down, out slopeHit,playerHeight/2 + sm.stepHeight))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
               
        }
        return false;
    }
    private void MoveOnSlope()
    {
        sm.direction = Vector3.ProjectOnPlane(sm.direction, slopeHit.normal);
        sm.rb.position -= new Vector3(0, slopeHit.distance, 0);
    }
    private void HandleStepClimb()
    {
        if (Physics.Raycast(sm.StepRayLower.transform.position, sm.transform.TransformDirection(Vector3.forward), .1f)
             && !Physics.Raycast(sm.StepRayUpper.transform.position, sm.transform.TransformDirection(Vector3.forward), .2f))
        {
            Debug.Log("climb up step");
            sm.rb.position += new Vector3(0f, sm.stepSmooth, 0f);
        }
        if (Physics.Raycast(sm.StepRayLower.transform.position, sm.transform.TransformDirection(Vector3.left),.1f)
            && !Physics.Raycast(sm.StepRayUpper.transform.position, sm.transform.TransformDirection(Vector3.left),.2f))
        {
            sm.rb.position += new Vector3(0f, sm.stepSmooth, 0f);
        }
        if (Physics.Raycast(sm.StepRayLower.transform.position, sm.transform.TransformDirection(Vector3.right),.1f)
            && !Physics.Raycast(sm.StepRayUpper.transform.position, sm.transform.TransformDirection(Vector3.right),.2f))
        {
            sm.rb.position += new Vector3(0f, sm.stepSmooth, 0f);
        }
        if (Physics.Raycast(sm.StepRayLower.transform.position, sm.transform.TransformDirection(Vector3.back),.1f)
            && !Physics.Raycast(sm.StepRayUpper.transform.position, sm.transform.TransformDirection(Vector3.back),.2f))
        {
            sm.rb.position += new Vector3(0f, sm.stepSmooth, 0f);
        }
    }
}


public class InCombat : Grounded
{
    public InCombat( Player stateMachine) : base("Combat", stateMachine)
    {
        sm = stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        sm.combat = true;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (sm.input.sprint || sm.input.interact) sm.ChangeState(sm.idleState);

        sm.timeSinceAttack += Time.deltaTime;
        HeavyAttack();
        QuickAttack();
        Block();
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        MoveRelativeToCamera();
    }
    public override void Exit()
    {
        base.Exit();
        sm.combat = false;
    }
   

    private void Block()
    {
        if (sm.input.block)
        {
            Debug.Log("Blocking");

            sm.blocking = true;
            sm._weaponR.tag = "Untagged";
            sm._weaponL.tag = "Untagged";
        }
        else
        {
            sm.blocking = false;
        }
    }
    private void HeavyAttack()
    {
        if (sm.input.heavyAttack && sm.timeSinceAttack > .8f)
        {
            Debug.Log("Strong Attack");

            sm.attack2 = true;

            sm.input.heavyAttack = false;
            sm._weaponR.tag = "Dmg";
            sm._weaponL.tag = "Dmg";
            //Reset Timer
            sm.timeSinceAttack = 0;
        }
        else
        {
            sm.attack2 = false;

            if (sm.timeSinceAttack > .8f)
            {
                sm._weaponR.tag = "Untagged";
                sm._weaponL.tag = "Untagged";
            }

        }
    }
    private void QuickAttack()
    {

        if (sm.input.quickAttack && sm.timeSinceAttack > .8f)
        {
            sm.attack1 = true;
            sm.currentAttack++;
            sm.input.quickAttack = false;
            sm._weaponR.tag = "Dmg";
            sm._weaponL.tag = "Dmg";

            Debug.Log("Quick Attack");

            //Reset Timer
            sm.timeSinceAttack = 0;
        }
        else
        {
            sm.attack1 = false;

            if (sm.timeSinceAttack > .8f)
            {
                sm._weaponR.tag = "Untagged";
                sm._weaponL.tag = "Untagged";
            }

        }
    }
    private void MoveRelativeToCamera()
    {
        float Verticalinput = sm.input.move.y;
        float HorizontalInput = sm.input.move.x;

        Vector3 forward = sm._mainCamera.transform.forward;
        Vector3 right = sm._mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        //move direction
        sm.direction = Verticalinput * forward + HorizontalInput * right;

        //Handle player rotation
        sm._targetRotation = sm._mainCamera.transform.eulerAngles.y;
         
        float rotation = Mathf.SmoothDampAngle(sm.transform.eulerAngles.y, sm._targetRotation, ref sm._rotationVelocity, sm.RotationSmoothTime);
        sm.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        sm._animator.SetFloat("VelocityZ", Verticalinput);
        sm._animator.SetFloat("VelocityX", HorizontalInput);

    }
}

public class OffCombat : Grounded
{
    public OffCombat(Player stateMachine) : base("Exploring", stateMachine)
    {
        sm = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (sm.input.heavyAttack || sm.input.quickAttack || sm.input.block) sm.ChangeState(sm.combatState);
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        FreeCameraMovement();
    }
    public override void Exit()
    {
        base.Exit();
    }
    
    private void handleAnimationBlend()
    {
        sm.motionSpeed = (sm.Inputmagnitude == 0) ? 10 : sm.Inputmagnitude;
        sm.animationBlend = Mathf.Lerp(sm.animationBlend, sm.targetSpeed, Time.deltaTime * sm.motionSpeed);
        if (sm.animationBlend < 0.01f) sm.animationBlend = 0f;
    }

    private void FreeCameraMovement()
    {
        HandleRotation();
        handleAnimationBlend();
        sm.direction = GetTargetDirection();
    }
    private Vector3 GetTargetDirection()
    {
        Vector3 inputDirection = new Vector3(sm.input.move.x, 0.0f, sm.input.move.y).normalized;

        sm._targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + sm._mainCamera.transform.eulerAngles.y;

        Vector3 targetDirection = Quaternion.Euler(0.0f, sm._targetRotation, 0.0f) * Vector3.forward;
        return targetDirection;
    }
    private void HandleRotation()
    {

        if (sm.input.move != Vector2.zero)
        {
            float rotation = Mathf.SmoothDampAngle(sm.transform.eulerAngles.y, sm._targetRotation, ref sm._rotationVelocity, sm.RotationSmoothTime);
            sm.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }
    
    
}

