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
    public bool pause;
    public bool menu;

    [Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
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

    public void OnPause(InputValue value)
    {
        PauseInput(value.isPressed);
    }
    public void OnMenu(InputValue value)
    {
        MenuInput(value.isPressed);
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

    public void QuickAttackInput(bool newAttackState)
    {
        quickAttack = newAttackState;
    }

    public void HeavyAttackInput(bool newAttackState)
    {
        heavyAttack = newAttackState;
    }

    public void BlockInput(bool newBlockState)
    {
        block = newBlockState;
    }
    public void EquipInput(bool newEquipState)
    {
        equip = newEquipState;
    }

    public void PauseInput(bool newPauseState)
    {
        pause = newPauseState;
    }
    public void MenuInput(bool newMenuState)
    {
        menu = newMenuState;
    }


    private void LateUpdate()
    {
		if (menu) menu = false;
		if (pause) pause = false;
		if (heavyAttack) heavyAttack = false;
		if (quickAttack) quickAttack = false;
		if (interact) interact = false;
		if (shapeshift) shapeshift = false;
		if (equip) equip = false;
    }
}

	
