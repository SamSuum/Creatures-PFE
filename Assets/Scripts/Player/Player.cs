using System;
using System.Collections.Generic;
using UnityEngine;

namespace PLAYER
{
    public class Player : PlayerStateMachine
    {        
        internal PlayerBaseState idleState;
        internal PlayerBaseState combatState;
        internal PlayerBaseState JumpingState;
        internal PlayerBaseState walkingState;
        internal PlayerBaseState sprintingState;
        internal PlayerBaseState fallingState;
        internal PlayerBaseState tiredState;

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

        [Header("Slope Movement")]
        public float maxSlopeAngle;
        public bool exitingSlope = false;
        public float slopeSpeed = 10f;

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
        internal readonly Collider[] _colliders = new Collider[3];
        internal IInteractable _interactable;
        public bool Interactable;

        [Header("ComBat")]
        public int currentAttack = 0;
        public GameObject _weaponL;
        public GameObject _weaponR;
        public float timeSinceAttack;

        [Header("Shapeshifting")]
        [SerializeField] private GameObject _defaultShape;
        public InteractionPromptUI _mimicPromptUI;
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
        [SerializeField] private float _defCamHeight = 2.0f;
        private bool canShapeShift;




        #endregion
        private void Awake()
        {
            idleState = new Idle(this);
            walkingState = new Walking(this);
            sprintingState = new Sprinting(this);
            combatState = new Combat(this);
            JumpingState = new Jumping(this);
            fallingState = new Falling(this);
            tiredState = new Tired(this);

            Init();

            if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
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
            UpdateDamageCoolDown(this);

            HandleShapeShift();

            if (HasAnimator()) UpdateAnimator(animationBlend, Inputmagnitude);

            if (dead) OnDeath(this);
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
        public void Heal(int amount)
        {
            HP.HealUnit(amount);
            stamina.MaxStamina = HP.Health;
            healthBar.SetHealth(HP.Health);
        }
        public void GainPsy(int amount)
        {
            GameManager.gameManager._playerPsy.RestoreUnit(amount);
            psyBar.SetPsy(GameManager.gameManager._playerPsy.Psy);
            Debug.Log(GameManager.gameManager._playerPsy.Psy);
        }


        #region Handle Physics
        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }
        public void HandleMovement()
        {
            Vector3 movement = direction * speed;

            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
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

        #region ShapeShifting
        private void HandleShapeShift()
        {            

            if (mimickable != null)
            {
                if (!_mimicPromptUI.isDisplayed)
                {
                    _mimicPromptUI.SetUp(mimickable.MimicPrompt);
                }
                canMimick = true;
            }
            else
            {
                if (_mimicPromptUI.isDisplayed) _mimicPromptUI.Close();
                canMimick = false;
                input.shapeshift = false;
            }

            canMimick = (canMimick && GameManager.gameManager._playerPsy.Psy > 0) ? true : false;

            canReset = (hasMimicked && canShapeShift) ? true : false;
            
            canShapeShift = (!canMimick && GameManager.gameManager._playerPsy.Psy > 0) ? true : false;

            if (input.shapeshift)
            {
                if (canMimick)
                {
                    Mimick();                            
                }
                if(canShapeShift)
                {
                    ShapeShift();
                }
                if (canReset)
                {
                    ResetShape();
                }
            }

            if (hasMimicked)
            {
                if (GameManager.gameManager._playerPsy.Psy <= 0) ResetShape();

                GameManager.gameManager._playerPsy.DecreaseUnit(1);
                psyBar.SetPsy(GameManager.gameManager._playerPsy.Psy);
            }
        }

        private void Mimick()
        {
            _targetShape = mimickable.GetShape(this);
            camTargetCoord = mimickable.GetCamCoord(this);

            ClearPrev(_shapeInstance);
            InitializePool();

            canMimick = false;
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
                CinemachineCameraTarget.transform.localPosition = new Vector3(0, _defCamHeight, 0);
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

            GetAnimator();
        }
        private void ReturnToPool(GameObject shapeInstance)
        {
            _shapePool.Enqueue(shapeInstance);
            shapeInstance.SetActive(false);
        }


        #endregion
    }

}

