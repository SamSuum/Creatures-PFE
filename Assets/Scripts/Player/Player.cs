using Cinemachine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.Windows;
#endif



/// <summary>
/// modified starter assets
/// Controls the player character : movement, combatn ... anything depending on input.
/// Notes : Needs optimizing; animation ID need to be in Shape script, a Finite state machine or a state pattern of some kind could be useful to implement all the actions (Exploration, Combat, Climbing, Swiming, etc...) 
/// </summary>



public class Player : MonoBehaviour
{
    #region Fields
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

   

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    [Tooltip("For adjusting camera sensitivity")]
    [SerializeField] private float _sensitivity = 60.0f;

    [Header("Shapeshifting")]
    [SerializeField] private GameObject _defaultShape;   
    [SerializeField] private int _psy = 100;    
    [SerializeField] private Queue<GameObject> _shapePool = new Queue<GameObject>();
    [SerializeField] private int _poolSize = 1;
    [SerializeField] private int _poolMaxSize = 2;
    public bool canMimick = false;
    public bool canReset = false;
    public bool transformed = false;
    public Vector3 camTargetCoord;
    
    private GameObject _targetShape;
    private GameObject _shapeInstance;
    

    [Header("Combat")]
    //Equip-Unequip parameters
    public bool isEquipping;    
    //Blocking Parameters
    public bool isBlocking;
    //Kick Parameters
    public bool isKicking;
    //Attack Parameters
    public bool isAttacking;
    public int currentAttack = 0;

    [SerializeField] private GameObject _weaponL;
    [SerializeField] private GameObject _weaponR;
   


    private float timeSinceAttack;

    [Header("Interactor")]

    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private bool _canMimick = false;

    private IMimickable _mimickable;
    private IInteractable _interactable;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
   

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDDeath;

    [HideInInspector]
    public Animator animator;
    private Rigidbody _rb;
    private PlayerInputs _input;
    private GameObject _mainCamera;
    private Hitbox _hitbox;

    private const float _threshold = 0.01f;   
    private bool _hasAnimator;

    #endregion

    #region properties


    #endregion

    #region runTime
    private void Awake()
    {           
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        animator = GetAnimator();
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInputs>();     
        _input = GetComponent<PlayerInputs>();
        _hitbox = GetComponentInChildren<Hitbox>();
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

        _targetShape = null;

    }

    private void Update()
    {
        //Health and damage
        if (GameManager.gameManager._playerHealth.Health == 0) Die();
        if (_hitbox.hit)
        { 
            animator.SetBool("Hit", true);
            PlayerTakeDmg(_hitbox.dmg);
        }
        else animator.SetBool("Hit", false);

        //Combat
        timeSinceAttack += Time.deltaTime;

        HeavyAttack();
        QuickAttack();
        Equip();
        Block();
       

        //Interaction
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0)
        {
            //interactions

            _interactable = _colliders[0].GetComponentInParent<IInteractable>();

            if (_interactable != null)
            {
                if (!_interactionPromptUI.isDisplayed)
                {
                    _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                }

                if (_input.interact)
                {
                    _interactable.Interact(this);
                    _input.interact = false;
                }

            }

            //Shapeshifting interactions

            _mimickable = _colliders[0].GetComponentInParent<IMimickable>();

            if (_mimickable != null)
            {
                _canMimick = true;
            }

        }
        else
        {
            if (_interactable == null) _interactable = null;
            if (_mimickable == null) _mimickable = null;

            if (_interactionPromptUI.isDisplayed) _interactionPromptUI.Close();

            _canMimick = false;
            _input.interact = false;
        }

        canMimick = ((CanMimick() || _shapePool.Count > 0) && _psy > 0) ? true : false;
        canReset = (transformed && !CanMimick()) ? true : false;

        if (_input.shapeshift)
        {
            if (canMimick)
            {
                ShapeShift();
                animator = GetAnimator();
                SetCamHeight();
            }
            if (canReset) 
            { 
                ResetShape();
                animator = GetAnimator();
                SetCamHeight();
            }

            _input.shapeshift = false;
        }

        if (transformed)
        {
            if (_psy > 0) _psy--;
            else ResetShape();
        }
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        GroundedCheck();
    }

    private void LateUpdate()
    {
        CameraRotation();
       
    }

   
    #endregion

    #region Movement  
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,transform.position.z);

        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            animator.SetBool(_animIDGrounded, Grounded);
        }
    }  
    private void Move()
    {

        UpdateVelocity();
        // move the player
        _rb.MovePosition(_rb.position+ UpdatedRotation().normalized * (_speed * Time.deltaTime));

        
    }

    private void UpdateVelocity()
    {
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;


        if (_input.move == Vector2.zero) targetSpeed = 0.0f;


        float currentHorizontalSpeed = new Vector3(_rb.velocity.x, 0.0f, _rb.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;


        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {

            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        UpdateAnimator(targetSpeed, inputMagnitude);
    }
    private Vector3 UpdatedRotation()
    {
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;


        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);


            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        return targetDirection;
    }
    
    private void Jump()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                animator.SetBool(_animIDJump, false);
                animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y);
                _rb.AddForce(Vector3.up * _verticalVelocity, ForceMode.VelocityChange);

                // update animator if using character
                if (_hasAnimator)
                {
                    animator.SetBool(_animIDJump, true);
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }
        
    }
    #endregion

    #region Camera
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += _input.look.x *_sensitivity* Time.deltaTime;
            _cinemachineTargetPitch += _input.look.y *_sensitivity* Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    private void SetCamHeight()
    {
        if (transformed)
            CinemachineCameraTarget.transform.localPosition = camTargetCoord;
        else
            CinemachineCameraTarget.transform.localPosition = new Vector3(0, 2.0f, 0);
    }
    #endregion

    #region Shapeshifting system
    private void ShapeShift()
    {
        _targetShape = GetMimickable().GetShape(this);
        camTargetCoord = GetMimickable().GetCamCoord(this);
        this.tag = "Bot";
        ClearPrev(_shapeInstance);
        InitializePool();
        ChangeShape();
       
    }

    private void InitializePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            _shapeInstance = Instantiate(_targetShape, transform);
            _shapeInstance.transform.position = transform.position;
            _shapePool.Enqueue(_shapeInstance);
            _shapeInstance.SetActive(false);
        }
    }

    private void ClearPrev(GameObject previous)
    {
        if (previous != null)
        {
            Destroy(previous.gameObject);
        }
    }

    private void ChangeShape()
    {
        if (_shapeInstance != null)
        {
            _defaultShape.SetActive(false);

            transformed = true;

            SetShape(_shapeInstance);
        }
        else Debug.Log("no object in memory");
    }

    private void ResetShape()
    {
        _defaultShape.SetActive(true);
        _defaultShape.tag = "Player";
        transformed = false;
       
        ReturnToPool(_shapeInstance);
        Destroy(_shapeInstance.gameObject);
       
    }

    private void ReturnToPool(GameObject shapeInstance)
    {
        _shapePool.Enqueue(shapeInstance);
        shapeInstance.SetActive(false);
    }

    private void SetShape(GameObject shapeInstance)
    {
        if (_shapePool.Count < _poolMaxSize)
        {
            shapeInstance = _shapePool.Dequeue();
            shapeInstance.SetActive(true);
        }
        else
        {
            shapeInstance.SetActive(true);
        }
    }

    public bool CanMimick()
    {
        return _canMimick;
    }

    public IMimickable GetMimickable()
    {
        return _mimickable;
    }


    //debug
    
    #endregion

    #region Combat System
    private void Equip()
    {
        if (_input.equip)
        {
            isEquipping = true;
            _input.equip = false;
            Debug.Log("Equipping");
        }
    }

    private void Block()
    {
        if (_input.block)
        {
            Debug.Log("Blocking");
            animator.SetBool("Block", true);
            _weaponR.tag = "Untagged";
            _weaponL.tag = "Untagged";
            isBlocking = true;            
        }
        else
        {
            animator.SetBool("Block", false);
            isBlocking = false;
        }
    }

    private void HeavyAttack()
    {
        if (_input.heavyAttack)
        {
            Debug.Log("Strong Attack");
            _weaponR.tag = "Dmg";
            _weaponL.tag = "Dmg";
            animator.SetBool("Attack1", true);
            isKicking = true;
            _input.heavyAttack = false;
        }
        else
        {
            animator.SetBool("Attack1", false);
            isKicking = false;
          
        }
    }

    private void QuickAttack()
    {

        if (_input.quickAttack && timeSinceAttack > 0.8f)
        {

            currentAttack++;
            isAttacking = true;
            _input.quickAttack = false;

            Debug.Log("Quick Attack");

            _weaponR.tag = "Dmg";
            _weaponL.tag = "Dmg";
            animator.SetBool("Attack2", true);
                    
            //Reset Timer
            timeSinceAttack = 0;
        }
        else
        {
            animator.SetBool("Attack2", false);
        }
    }


    #endregion

    #region Health & Damage System
    private void PlayerTakeDmg(int dmg)
    {
        GameManager.gameManager._playerHealth.DmgUnit(dmg);
    }
    private void PlayerHeal(int healing)
    {
        GameManager.gameManager._playerHealth.HealUnit(healing);
        Debug.Log(GameManager.gameManager._playerHealth.Health);

    }

    private void Die()
    {
        animator.SetTrigger(_animIDDeath);
        _mainCamera.GetComponent<CinemachineBrain>().enabled = false;
        Destroy(this);
    }
    #endregion

    #region Animation
    private void UpdateAnimator(float targetSpeed, float inputMagnitude)
    {
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
        // update animator 
        if (_hasAnimator)
        {
            animator.SetFloat(_animIDSpeed, _animationBlend);
            animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }
    private void AssignAnimationIDs()
    {
        _animIDDeath = Animator.StringToHash("Dead");
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public Animator GetAnimator()
    {

        foreach (Transform child in this.gameObject.transform)
        {
            if (child.CompareTag("Player") && child.gameObject.activeSelf)
            {
                _hasAnimator = true;
                return child.GetComponent<Animator>();
            }
        }
        _hasAnimator = false;
        return null;

    }
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
    #endregion

    

}
