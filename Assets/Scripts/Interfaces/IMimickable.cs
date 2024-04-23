using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IMimickable
{
    public string MimicPrompt { get; }
    public GameObject ObjectShape { get; }
    public Vector3 CamCoord { get; }
    public Animator anim { get; }
    public GameObject GetShape(Actor player);
    public Vector3 GetCamCoord(Actor player);
    public Animator GetNewAnimator(Actor player);

}

