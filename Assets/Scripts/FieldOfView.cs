using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private float _radius;
    [SerializeField, Range(0, 360)]
    private float _angle;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private LayerMask _targetMask;
    [SerializeField]
    private LayerMask _obstacleMask;

    private bool _canSeePlayer;

    public bool CanSeePlayer => _canSeePlayer;
    public float Radius => _radius;
    public float Angle => _angle;
    public Player Player => _player;

    private void Start()
    {
        StartCoroutine(FovRoutine());
    }
    private IEnumerator FovRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
    private void FieldOfViewCheck()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(
            transform.position, _radius, _targetMask
            );

        List<Collider2D> validObstacles = new List<Collider2D>();
        foreach (var collider in rangeChecks)
        {
            if ( collider.gameObject != this.gameObject )
                validObstacles.Add( collider );
        }
        
        if (validObstacles.Count > 0)
        {
            Transform target = validObstacles[0].transform;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.right, directionToTarget) < _angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);               
                RaycastHit2D[] hits = Physics2D.RaycastAll(
                    transform.position, directionToTarget, distanceToTarget, _obstacleMask
                    );
                
                bool hitObstacle = false;
                foreach (var hit in hits)
                {
                    if (hit.collider != null && hit.collider.gameObject != this.gameObject ) 
                    { hitObstacle = true; 
                        break; }
                }
                if (!hitObstacle)
                {
                    print("Found no obstacles");
                    _canSeePlayer = true;
                }
                else {
                    _canSeePlayer = false;}
            }
            else {_canSeePlayer= false;}
        }
        else if (_canSeePlayer) {_canSeePlayer=false;}
    }
}
