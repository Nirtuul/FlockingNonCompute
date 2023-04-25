using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int flockSize = 20;

    [SerializeField]
    private float speedModifier = 5;

    [SerializeField]
    private float alignmentWeight = 1;

    [SerializeField]
    private float cohesionWeight = 1;

    [SerializeField]
    private float separationWeight = 1;

    [SerializeField]
    private float followWeight = 5;

    [Header("Boid Data")] 
    [SerializeField]
    private boid prefab;
    [SerializeField]
    private float spawnRadius = 3.0f;
    private Vector3 spawnLocation = Vector3.zero;

    
    [Header("Target Data")]
    [SerializeField]
    public Transform target;
    private Vector3 flockCenter;
    private Vector3 flockDirection;
    private Vector3 targetDirection;
    private Vector3 separation;
    public List<boid> flockList = new List<boid>();
    public float SpeedModifier { get { return speedModifier; } }
    
    private void Awake()
    {
        flockList = new List<boid>(flockSize); 
        for (int i = 0; i < flockSize; i++)
        {
            spawnLocation = Random.insideUnitSphere * spawnRadius + transform.position;
            boid boid = Instantiate(prefab, spawnLocation, transform.rotation) as boid;

            boid.transform.parent = transform;
            boid.FlockController = this;
            flockList.Add(boid);
        }
    }
    public Vector3 Flock(boid boid, Vector3 boidPosition, Vector3 boidDirection)
    {
        flockDirection = Vector3.zero;
        flockCenter = Vector3.zero;
        targetDirection = Vector3.zero;
        separation = Vector3.zero;

        for (int i = 0; i < flockList.Count; ++i) 
        {
            boid neighbor = flockList[i];
            //Check only against neighbors.
            if (neighbor != boid) 
            {
                //Aggregate the direction of all the boids.
                flockDirection += neighbor.Direction;
                //Aggregate the position of all the boids.
                flockCenter += neighbor.transform.localPosition;
                //Aggregate the delta to all the boids.
                separation += neighbor.transform.localPosition - boidPosition;
                separation *= -1;
            }
        }
        //Alignment. The average direction of all boids.
        flockDirection /= flockSize;
        flockDirection = flockDirection.normalized * alignmentWeight;

        //Cohesion. The centroid of the flock.
        flockCenter /= flockSize;
        flockCenter = flockCenter.normalized * cohesionWeight;

        //Separation.
        separation /= flockSize;
        separation = separation.normalized * separationWeight;

        //Direction vector to the target of the flock.
        targetDirection = target.localPosition - boidPosition;
        targetDirection = targetDirection * followWeight;

        return flockDirection + flockCenter + separation + targetDirection;
    }



}
