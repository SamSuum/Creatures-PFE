using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ai
{
    public class PatrolState : AI_State
    {
        bool isScanning = false;
        private Transform currentwaypoint;

        public override void OnEnter(AI_Data data)
        {
            Waypoints wp = data.waypoints;
            Transform sc = data.transform;

            //initial position
            currentwaypoint = wp.GetNextWaypoint(currentwaypoint);
            sc.position = currentwaypoint.position;

            //next target position
            SetNextWaypoint(sc, wp);
        }

        public override void OnUpdate(AI_Data data, MonoBehaviour monoBehaviour)
        {
            Waypoints wp = data.waypoints;
            Transform sc = data.transform;
            Transform scanner = data.scanner;
            float speed = data.speed;            
            float dist = data.dist;
            float rotationSpeed = data.rotationSpeed;
            int n = data.n;

            if (Vector3.Distance(sc.position, currentwaypoint.position) < dist)
            {                
                if(scanner!=null) monoBehaviour.StartCoroutine(UseScanner(scanner, rotationSpeed, n, sc));

                currentwaypoint = wp.GetNextWaypoint(currentwaypoint);
            }
           
            if(!isScanning) sc.position = Vector3.MoveTowards(sc.position, currentwaypoint.position, speed * Time.deltaTime);
        }

        private void SetNextWaypoint(Transform sc, Waypoints wp)
        {
            currentwaypoint = wp.GetNextWaypoint(currentwaypoint);
            sc.rotation = Quaternion.LookRotation(sc.position - currentwaypoint.position);
        }

        private IEnumerator UseScanner(Transform scanner,float rotationSpeed, int n,Transform sc)
        {
            sc.Rotate(Vector3.up*-90);

            isScanning = true;
            scanner.gameObject.SetActive(true);

            for (int i=0; i<n; i++)
            {                               
                sc.Rotate(Vector3.right, rotationSpeed);
                yield return new WaitForSeconds(.1f);
            }    
            
            scanner.gameObject.SetActive(false);
            isScanning = false;
            sc.rotation = Quaternion.LookRotation(sc.position - currentwaypoint.position);
            yield return null;
            
        }
        
    }
}
    

