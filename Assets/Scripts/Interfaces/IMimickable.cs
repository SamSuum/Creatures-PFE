using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IMimickable
{
    public GameObject ObjectShape { get; }
    public Vector3 CamCoord { get; }
    public GameObject GetShape(Player player);
    public Vector3 GetCamCoord(Player player);

}

