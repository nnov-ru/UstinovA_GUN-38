using UnityEngine;
using UnityEngine.AI;

public class Search
{
    private AIMotion _controller;
    private Transform _closestSphere;
    private float _currentSearchTimer;
    private Vector3 _searchDestination;
    public Search(AIMotion controller)
    {  
        _controller = controller;
    }
    public Transform closestSphere => _closestSphere;
    public void Enter()
    {
        _controller.agent.isStopped = false;
        _controller.SetAnimation("Search");
        _closestSphere = null;
        _currentSearchTimer = 0f;
    }
    public void Update()
    {
        if (_closestSphere == null) FindClosestSphere();
        if (_closestSphere != null)
        {
            _controller.agent.SetDestination(_closestSphere.position);
            
            if (Vector3.Distance(_controller.transform.position, _closestSphere.position) <= _controller.pickupDistance)
            {
                _controller.ChangeState(AIState.Collect, _closestSphere);
                return;
            }
        }
        else
        {
            _currentSearchTimer += Time.deltaTime;
            if (_currentSearchTimer >= _controller.searchMoveTime || _controller.agent.remainingDistance < .5f)
            {
                MoveToRandom();
                _currentSearchTimer = 0f;
            }
        }
    }
    public void FindClosestSphere()
    {
        Transform closestSphere = null;
        float closestDistance = Mathf.Infinity;
        foreach (Transform sphere in _controller.availableSpheres)
        {
            if (sphere == null || !sphere.gameObject.activeSelf) continue;

            float distance = Vector3.Distance(_controller.transform.position, sphere.position);

            if (distance <= _controller.searchRadius && distance < closestDistance)
            {
                closestDistance = distance;
                closestSphere = sphere;
            }
        }
        _closestSphere = closestSphere;
    }
    private void MoveToRandom()
    {
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += _controller.transform.position;
            randomDirection.y += _controller.transform.position.y;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                _searchDestination = hit.position;
                _controller.agent.SetDestination(_searchDestination);
            }
    }
    public void SetTarget(Transform closestSphere)
    { _closestSphere = closestSphere; }
    public Transform GetTarget()
    { return _closestSphere; }
}
