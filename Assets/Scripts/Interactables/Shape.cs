using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour, IMimickable
{
    [SerializeField] private string _prompt;
    [SerializeField] private GameObject _shape;
    [SerializeField] private Vector3 _camCoord;
    [SerializeField] private Animator _anim;

    public string InteractionPrompt => _prompt;
    public GameObject ObjectShape => _shape;
    public Animator anim => _anim;
    public Vector3 CamCoord => _camCoord;

    public string MimicPrompt => _prompt;

    public  GameObject GetShape(Actor player)
    {
        return _shape;
    }
    public Vector3 GetCamCoord(Actor player)
    {
        return _camCoord;
    }

    public Animator GetNewAnimator(Actor player)
    {
        return _anim;
    }
}
