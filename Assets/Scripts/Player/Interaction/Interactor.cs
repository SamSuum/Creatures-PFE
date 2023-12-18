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
    [SerializeField] private InteractionPromptUI _interactionPromptUI;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private bool _canMimick = false;

    private IMimickable _mimickable;
    private IInteractable _interactable;
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

            _interactable = _colliders[0].GetComponentInParent<IInteractable>();           

            if (_interactable != null)
            {
                if(!_interactionPromptUI.isDisplayed)
                {
                    _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                }

                if(_input.interact)
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
