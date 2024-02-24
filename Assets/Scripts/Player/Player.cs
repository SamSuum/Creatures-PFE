using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : PlayerStateMachine
{
    [HideInInspector]
    public OffCombat idleState;
    [HideInInspector]
    public InCombat combatState;
    [HideInInspector]
    public InAir inAirState;
    #region fields
    public PlayerInputs input;

    [Header("Animation Blend")]
    public float animationBlend;
    public float targetSpeed;
    [HideInInspector]
    public float Inputmagnitude;
    public float motionSpeed;

    [Header("GroundCheck")]
    public float GroundedRadius = .28f;
    public LayerMask GroundLayers;
    public float GroundedOffset = -.14f;

    [Header("Speed")]
    public float SprintSpeed = 60.0f;
    public float MoveSpeed = 15.0f;
    public float speed;
    public float SpeedChangeRate = 10.0f;
    public Vector3 direction;

    [Header("Jump")]
    public float jumpTimeoutDelta;
    public float JumpTimeout = .50f;
    public float fallTimeoutDelta;
    public float FallTimeout = .15f;
    public float verticalVelocity;
    public float terminalVelocity;
    public float JumpHeight = .02f;

    [Header("Camera")]
    public GameObject _mainCamera;
    public float _targetRotation;
    public float _rotationVelocity;
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = .12f;
    // cinemachine
    public float _cinemachineTargetYaw;
    public float _cinemachineTargetPitch;
    public const float _threshold = 0.01f;
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;
    public float _sensitivity = 60.0f;


    [Header("Interactor")]
    public LayerMask _interactableMask;
    public InteractionPromptUI _interactionPromptUI;
    public int _numFound;
    public Transform interactionPoint;
    public float interactionPointRadius = 0.5f;
    private readonly Collider[] _colliders = new Collider[3];
    private IInteractable _interactable;

    [Header("ComBat")]
    public int currentAttack = 0;
    public GameObject _weaponL;
    public GameObject _weaponR;
    public float timeSinceAttack;

    [Header("Shapeshifting")]
    [SerializeField] private GameObject _defaultShape;

    [SerializeField] private Queue<GameObject> _shapePool = new Queue<GameObject>();
    [SerializeField] private int _poolSize = 1;
    [SerializeField] private int _poolMaxSize = 2;

    public bool hasMimicked = false;
    public bool canMimick = false;
    public bool canReset = false;

    public IMimickable mimickable;

    public Vector3 camTargetCoord;
    [SerializeField] private GameObject _targetShape;
    [SerializeField] private GameObject _shapeInstance;


    [Header("Stairs handling")]
    public GameObject StepRayUpper;
    public GameObject StepRayLower;
    public float stepHeight = .3f;
    public float stepSmooth = .1f;


    #endregion
    private void Awake()
    {
        idleState = new OffCombat(this);
        combatState = new InCombat(this);        
        inAirState = new InAir(this);

        Init();

        if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        StepRayUpper.transform.position += new Vector3(0, stepHeight, 0);
    }

    public override void OnStart()
    {
        HP = GameManager.gameManager._playerHealth;

        GameEvents.current.onHitTriggerEnter += OnHitTaken;

        GameEvents.current.onHitTriggerExit += OnHitRecover;

        stamina = GameManager.gameManager._playerStamina;

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

       
    }
    public override void OnFixedUpdate()
    {
        HandleMovement();
        GroundedCheck();
    }
    public override void OnUpdate()
    {      
        HandleInteractions();
        HandleShapeShift();
        if (HasAnimator()) UpdateAnimator(animationBlend, Inputmagnitude);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    protected override PlayerBaseState GetInitState()
    {
        return idleState;
    }

    private void OnDestroy()
    {
        GameEvents.current.onHitTriggerEnter -= OnHitTaken;
        GameEvents.current.onHitTriggerExit -= OnHitRecover;
    }

    #region Handle Physics
    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }
    public void HandleMovement()
    {
        rb.MovePosition(rb.position + direction.normalized * speed * Time.deltaTime);
    }
    #endregion

    #region Handle Camera
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += input.look.x * _sensitivity * Time.deltaTime;
            _cinemachineTargetPitch += input.look.y * _sensitivity * Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }
    #endregion

    private void HandleInteractions()
    {
        //Interaction
        _numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, _colliders, _interactableMask);

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

                if (input.interact)
                {
                    _interactable.Interact(this);
                    input.interact = false;
                }
            }
            mimickable = _colliders[0].GetComponentInParent<IMimickable>();

            if (mimickable != null)
            {
                canMimick = true;
            }
            else canMimick = false;

        }
        else
        {
            if (_interactable == null) _interactable = null;
            if (_interactionPromptUI.isDisplayed) _interactionPromptUI.Close();
            input.interact = false;
        }
    }

    private void HandleShapeShift()
    {
        canMimick = ( canMimick && GameManager.gameManager._playerPsy.Psy > 0 ) ? true : false;

        canReset = (hasMimicked && canMimick) ? true : false;

        if (input.shapeshift)
        {
            if (canMimick)
            {
                Mimick();
                if (_shapeInstance != null)
                {
                    ShapeShift();
                }
            }
            if (canReset)
            {
                ResetShape(); 
            }
            input.shapeshift = false;
        }

        if (hasMimicked)
        {
            if (GameManager.gameManager._playerPsy.Psy <= 0) ResetShape();

            GameManager.gameManager._playerPsy.DecreaseUnit(1);
            psyBar.SetPsy(GameManager.gameManager._playerPsy.Psy);
        }
    }

    #region ShapeShifting

    private void Mimick()
    {
        _targetShape = mimickable.GetShape(this);
        camTargetCoord = mimickable.GetCamCoord(this);
        ClearPrev(_shapeInstance);
        InitializePool();
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
    

    private void ShapeShift()
    {
        this.tag = "Bot";
        ChangeShape();
        SetCamHeight();
        GetAnimator();
    }
    private void SetCamHeight()
    {
        if (hasMimicked)
            CinemachineCameraTarget.transform.localPosition = camTargetCoord;
        else
            CinemachineCameraTarget.transform.localPosition = new Vector3(0, 2.0f, 0);
    }
    private void ChangeShape()
    {
        if (_shapeInstance != null)
        {
            _defaultShape.SetActive(false);

            hasMimicked = true;

            SetShape(_shapeInstance);
        }
        else Debug.Log("no object in memory");
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

   

    private void ResetShape()
    {
        _defaultShape.SetActive(true);
        _defaultShape.tag = "Player";
        hasMimicked = false;

        SetCamHeight();
        ReturnToPool(_shapeInstance);
        Destroy(_shapeInstance.gameObject);
        _shapeInstance = null;
        GetAnimator();
    }
    private void ReturnToPool(GameObject shapeInstance)
    {
        _shapePool.Enqueue(shapeInstance);
        shapeInstance.SetActive(false);
    }

   
    #endregion
}

