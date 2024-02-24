using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour, IInteractable,IMimickable
{
    [SerializeField] private string _prompt;
    [SerializeField] private GameObject _shape;
    [SerializeField] private Vector3 _camCoord;
    [SerializeField] private Animator _anim;

    public string InteractionPrompt => _prompt;
    public GameObject ObjectShape => _shape;
    public Animator anim => _anim;
    public Vector3 CamCoord => _camCoord;

    public bool Interact(Actor player)
    {
        Debug.Log("You can transform into this");
        return true;
    }

    public  GameObject GetShape(Player player)
    {
        return _shape;
    }
    public Vector3 GetCamCoord(Player player)
    {
        return _camCoord;
    }

    public Animator GetNewAnimator(Player player)
    {
        return _anim;
    }
}
