using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour, IInteractable,IMimickable
{
    [SerializeField] private string _prompt;
    [SerializeField] private GameObject _shape;
    [SerializeField] private Vector3 _camCoord;

    public string InteractionPrompt => _prompt;
    public GameObject ObjectShape => _shape;

    public Vector3 CamCoord => _camCoord;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("You can transform into this");
        return true;
    }

    public  GameObject GetShape(ShapeShifter shapeShifter)
    {
        return _shape;
    }
    public Vector3 GetCamCoord(ShapeShifter shapeShifter)
    {
        return _camCoord;
    }
}
