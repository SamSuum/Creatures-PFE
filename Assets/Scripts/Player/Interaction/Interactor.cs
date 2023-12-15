using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    //ref
    private PlayerInputs _input;

    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private bool _canMimick = false;
    private IMimickable _mimickable;

    private void Awake()
    {
        _input = GetComponent<PlayerInputs>();
    }

    private void Update()
    {
        //Interaction
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        if (_numFound > 0)
        {
            //interactions

            var interactable = _colliders[0].GetComponentInParent<IInteractable>();           

            if (interactable != null && _input.interact)
            {
                interactable.Interact(this);
                _input.interact = false;
            }

            //Shapeshifting interactions

            _mimickable = _colliders[0].GetComponentInParent<IMimickable>();

            if (_mimickable != null)
            {
                _canMimick = true;
               

            }
            else _canMimick = false;

        }
        else
        {
            _input.interact = false;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
