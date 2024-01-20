using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif



public class PlayerInputs : MonoBehaviour
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;
	public bool interact;
	public bool shapeshift;
    public bool quickAttack;
    public bool heavyAttack;
    public bool block;
    public bool equip;

    [Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
	public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		if (cursorInputForLook)
		{
			LookInput(value.Get<Vector2>());
		}
	}

	public void OnJump(InputValue value)
	{
		JumpInput(value.isPressed);
	}

	public void OnSprint(InputValue value)
	{
		SprintInput(value.isPressed);
	}
	public void OnInteract(InputValue value)
	{
		InteractInput(value.isPressed);
	}
	public void OnShapeshift(InputValue value)
	{
		ShapeshiftInput(value.isPressed);
	}

    public void OnQuickAttack(InputValue value)
    {
        QuickAttackInput(value.isPressed);
    }
    public void OnHeavyAttack(InputValue value)
    {
        HeavyAttackInput(value.isPressed);
    }
    public void OnBlock(InputValue value)
    {
        BlockInput(value.isPressed);
    }
    public void OnEquip(InputValue value)
    {
        EquipInput(value.isPressed);
    }
#endif


    public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	public void InteractInput(bool newInteractState)
	{
		interact = newInteractState;
	}
	public void ShapeshiftInput(bool newShapeshiftState)
    {
		shapeshift = newShapeshiftState;
    }

    public void QuickAttackInput(bool newShapeshiftState)
    {
        quickAttack = newShapeshiftState;
    }

    public void HeavyAttackInput(bool newShapeshiftState)
    {
        heavyAttack = newShapeshiftState;
    }

    public void BlockInput(bool newShapeshiftState)
    {
        block = newShapeshiftState;
    }
    public void EquipInput(bool newShapeshiftState)
    {
        equip = newShapeshiftState;
    }
    private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}

	
