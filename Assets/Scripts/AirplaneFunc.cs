using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AirplaneFunc : MonoBehaviour
{
    public Airplane planeType;

    public NavMeshAgent agent;

    public GameObject lights;

    public float stopDistance = 0.25f;

    public float length = 16, width = 8;

    private Vector3 movementTarget;

    private bool roaming = true;

    [HideInInspector]
    public bool parked;

    void Start()
    {
        SetTarget(length, width);
    }

    void Update()
    {
        if ((Vector3.Distance(movementTarget, transform.position) <= stopDistance) && roaming) SetTarget(length, width);
    }

    public void GoToPoint(Vector3 point)
    {
        agent.SetDestination(point);
        roaming = false;
    }

    public void SetTarget(float length, float width)
    {
        movementTarget = GetTarget(length, width);
        agent.SetDestination(movementTarget);
    }

    public Vector3 GetTarget(float length, float width)
    {
        return new Vector3(Random.Range(0 - length / 2, 0 + length / 2), 0, Random.Range(0 - width / 2, 0 + width / 2));
    }
}
