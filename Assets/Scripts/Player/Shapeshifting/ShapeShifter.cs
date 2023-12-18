using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.Windows;

public class ShapeShifter : MonoBehaviour
{
    [SerializeField] private GameObject _defaultShape;
    private GameObject _targetShape;
    private GameObject _shapeInstance;

    [SerializeField] private int _psy = 100;

    public bool transformed = false;
    public Vector3 camTargetCoord;

    private Queue<GameObject> _shapePool = new Queue<GameObject>();
    [SerializeField] private int _poolSize = 1;
    [SerializeField] private int _poolMaxSize = 2;

    private Interactor _interactor;
    private PlayerInputs _input;

    public bool canMimick = false;
    public bool canReset = false;


    // Start is called before the first frame update


    // Update is called once per frame
    private void Awake()
    {
        _interactor = GetComponent<Interactor>();
        _input = GetComponent<PlayerInputs>();
       
    }

    private void Start()
    {
        _targetShape = null;

    }
    void Update()
    {
       canMimick = ( (_interactor.CanMimick() || _shapePool.Count > 0) && _psy>0 ) ? true : false;
       canReset  = (transformed && !_interactor.CanMimick()) ? true : false;

        if (_input.shapeshift)
        { 
            if(canMimick) ShapeShift();
            if(canReset) ResetShape();

            _input.shapeshift = false;
        }

        if(transformed)
        {
            if (_psy > 0) _psy--;
            else ResetShape();
        }
      
    }

    private void ShapeShift()
    {
        _targetShape = _interactor.GetMimickable().GetShape(this);
        camTargetCoord = _interactor.GetMimickable().GetCamCoord(this);

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
}
