using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class DebugNMA : MonoBehaviour
{
    [SerializeField] private bool _velocity;
    [SerializeField] private bool _desiredVelocity;
    [SerializeField] private bool _path;

    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    private void OnDrawGizmos()
    {
        if (_velocity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position+ _agent.velocity);
        }
        if (_desiredVelocity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position+ _agent.desiredVelocity);
        }
        if (_path) 
        { 
            Gizmos.color = Color.black;
            var agentPath = _agent.path;
            Vector3 prevCorner = transform.position;

            foreach (var corner in agentPath.corners)
            {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawSphere(corner, .1f);
                prevCorner = corner;
            }

        }
    }
}
