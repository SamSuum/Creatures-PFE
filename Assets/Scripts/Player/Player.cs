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

        [Header("Shapeshifting")]
        [SerializeField] private GameObject _defaultShape;
        public InteractionPromptUI _mimicPromptUI;

        [SerializeField] private Queue<GameObject> _shapePool = new Queue<GameObject>();
        [SerializeField] private int _poolSize = 1;
        [SerializeField] private int _poolMaxSize = 2;

        [SerializeField] private List<GameObject> _Shapes = new List<GameObject>();
        [SerializeField] private GameObject _ShapeShiftingUI ; //drop down menu near health bar holding shapeshifting key

        public bool hasMimicked = false;
        public bool canMimick = false;
        public bool canReset = false;
        public bool canShapeShift;

        public IMimickable mimickable;
        public Vector3 camTargetCoord;
        [SerializeField] private GameObject _targetShape;
        [SerializeField] private GameObject _shapeInstance;
        [SerializeField] private float _defCamHeight = 2.0f;

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
        public RaycastHit groundHit;

        [Header("Slope Movement")]
        public float maxSlopeAngle;
        public bool exitingSlope = false;


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
        public CameraHandler cameraHandler;


        [Header("Interactor")]
        public LayerMask _interactableMask;
        public InteractionPromptUI _interactionPromptUI;
        public int _numFound;
        public Transform interactionPoint;
        public float interactionPointRadius = 0.5f;
        internal readonly Collider[] _colliders = new Collider[3];
        internal IInteractable _interactable;
        public bool Interactable;

        [Header("Combat")]
        public int currentAttack = 0;
        public GameObject _weaponL;
        public GameObject _weaponR;
        public float timeSinceAttack;

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
        }

        public override void OnStart()
        {
            HP = GameManager.gameManager._playerHealth;

            GameEvents.current.onHitTriggerEnter += OnHitTaken;

            GameEvents.current.onHitTriggerExit += OnHitRecover;

            stamina = GameManager.gameManager._playerStamina;
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

        protected override PlayerBaseState GetInitState()
        {
            return idleState;
        }

        private void OnDestroy()
        {
            GameEvents.current.onHitTriggerEnter -= OnHitTaken;
            GameEvents.current.onHitTriggerExit -= OnHitRecover;
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
            grounded = Physics.Raycast(spherePosition, Vector3.down, out groundHit, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
           
        }
        public void HandleMovement()
        {
            Vector3 movement = direction * speed;

            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
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
            }


            canReset = (hasMimicked) ? true : false;
            
            canShapeShift = (GameManager.gameManager._playerPsy.Psy > 0 && _targetShape!=null && !hasMimicked && !canMimick) ? true : false;

            if (input.shapeshift)
            {
                if (canMimick)
                {
                    Mimick();
                }
                if (canReset)
                {
                    ResetShape();
                }
                if (canShapeShift)
                {
                    ShapeShift();
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

            /// add to list if space available 
            /// else overwrite last element 
            /// or open menu to choose an element to overwrite (default cannot be overwritten)

            canMimick = false;
        }

        private void InitializeShapeList()
        {
           //put default in index zero

           // add (maxsize) empty elements 
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
            /// deactivate or destroy current shape
            /// instantiate or activate new shape
            /// unless it's null


            this.tag = "Bot";
            ChangeShape();
            SetCamHeight();
            GetAnimator();
        }
        private void SetCamHeight()
        {
            if (hasMimicked)
                cameraHandler.CinemachineCameraTarget.transform.localPosition = camTargetCoord;
            else
                cameraHandler.CinemachineCameraTarget.transform.localPosition = new Vector3(0, _defCamHeight, 0);
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

