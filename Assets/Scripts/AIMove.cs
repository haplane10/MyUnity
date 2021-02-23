using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] List<Transform> transforms;
    [SerializeField] Transform target;
    private void Start()
    {
        target = GetRandomPosition();
        navMeshAgent.SetDestination(target.position);
    }

    void Update()
    {
        Debug.Log(target.name);
        Debug.Log(navMeshAgent.remainingDistance != float.PositiveInfinity && navMeshAgent.remainingDistance < 0.1f);
        if (navMeshAgent.remainingDistance < 0.2f)
        {
            var target = GetRandomPosition();
            navMeshAgent.SetDestination(target.position);
        }
    }

    private Transform GetRandomPosition()
    {
        var idx = Random.Range(0, transforms.Count);
        return transforms[idx];
    }
 }