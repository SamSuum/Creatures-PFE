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

            float speed = data.speed;
            
            float dist = data.dist;

            Transform scanner = data.scanner;
            Transform pivot = data.pivot; 

            float rotationSpeed = data.rotationSpeed;

            Vector3 rotationThreshold = data.rotationThreshold; 

            int n = data.n;

            if (Vector3.Distance(sc.position, currentwaypoint.position) < dist)
            {
                
                monoBehaviour.StartCoroutine(UseScanner(scanner, pivot, rotationSpeed, rotationThreshold, n, sc,wp));
                currentwaypoint = wp.GetNextWaypoint(currentwaypoint);
            }
           
            if(!isScanning) sc.position = Vector3.MoveTowards(sc.position, currentwaypoint.position, speed * Time.deltaTime);
        }

        private void SetNextWaypoint(Transform sc, Waypoints wp)
        {
            currentwaypoint = wp.GetNextWaypoint(currentwaypoint);
            sc.rotation = Quaternion.LookRotation(sc.position - currentwaypoint.position);
        }

        private IEnumerator UseScanner(Transform scanner,Transform pivot,float rotationSpeed,Vector3 rotationThreshold, int n,Transform sc, Waypoints wp)
        {
            sc.Rotate(Vector3.up*-90);
            isScanning = true;
            scanner.gameObject.SetActive(true);

            for (int i=0; i<n; i++)
            {
                if (scanner.eulerAngles == rotationThreshold)
                {
                    rotationSpeed *= -1;
                    rotationThreshold *= -1;
                }
                
                sc.Rotate(Vector3.right, rotationSpeed);
                yield return new WaitForSeconds(.2f);
            }

            scanner.eulerAngles = Vector3.zero;
            scanner.gameObject.SetActive(false);
            isScanning = false;
            sc.rotation = Quaternion.LookRotation(sc.position - currentwaypoint.position);
            yield return null;

            
        }
        
    }
}
    

