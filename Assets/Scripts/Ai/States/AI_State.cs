using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Ai
{
    [System.Serializable]
    public struct AI_Data
    {
        [Header("Waypoints data")]
        public Waypoints waypoints;
        public Transform transform;
        public float speed;
        public float dist;

        [Header("Scanner")]
        public Transform scanner;        
        public float rotationSpeed;       
        public int n;
    }

    public abstract class AI_State
    {
       
        public virtual void OnEnter(AI_Data data)
        {    
        }

        public virtual void OnUpdate(AI_Data data, MonoBehaviour monoBehaviour)
        { 
        }

        public virtual void OnExit()
        {  
        }
        
    }
}



