using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GroundAI : EnemyAI
{
    public NavMeshAgent myNavMeshAgent;

    public override void Initialize()
    {
        myNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }
 
}
