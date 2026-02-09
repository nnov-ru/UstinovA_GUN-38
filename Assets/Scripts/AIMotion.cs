using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class AIMotion : MonoBehaviour
{
    [SerializeField] private List<Transform> _allSpheres = new List<Transform>(); // Все 8 сфер
    [SerializeField] private float _pickupDistance = 1.5f;
    [SerializeField] private float _searchRadius = 5f;
    //[SerializeField] private bool _debugMode = true;
    [SerializeField] private float _idleTime = 5f;
    [SerializeField] private float _searchMoveTime = 3f;

    private NavMeshAgent _agent;
    private Transform _currentTargetSphere;
    private List<Transform> _availableSpheres = new List<Transform>(); // Доступные сферы
    private Animator _animator;

    public enum AIState { Idle, Search, Collect }
    private AIState _currentState = AIState.Search;
    private float _stateTimer = 0f;
    private Vector3 _searchDestination;
    private float _currentSearchTimer = 0f;
    private bool _collecting = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        if (_allSpheres.Count == 0)
        {
            Debug.LogWarning($"No balls in list in Inspector !");
            return;
        }

        _availableSpheres = new List<Transform>(_allSpheres);
        ChangeState(AIState.Search);
    }

    private void Update()
    {
        if (_availableSpheres.Count == 0)
        {
            _agent.isStopped = true;
            SetAnimation("Idle");
            this.enabled = false;
            Debug.Log($"All balls collected !");
            return;
        }
        _stateTimer += Time.deltaTime;
        switch (_currentState)
        {
                case AIState.Idle:
                UpdateIdle();
                break;
                case AIState.Search:
                UpdateSearch();
                break;
                case AIState.Collect:
                UpdateCollect();
                break;
        }
    }
    private void UpdateIdle()
    {
        if (_stateTimer >= _idleTime)
            ChangeState(AIState.Search);
    }
    private void UpdateSearch()
    {
        if (_currentTargetSphere == null)
        {
            FindClosestSphere(_searchRadius);
        }
        if (_currentTargetSphere != null)
        {
            _agent.SetDestination(_currentTargetSphere.position);
            float distance = Vector3.Distance(transform.position, _currentTargetSphere.position);
            if (distance <= _pickupDistance)
            {
                ChangeState(AIState.Collect);
                return;
            }
        }
        else
        {
            _currentSearchTimer += Time.deltaTime;
            if (_currentSearchTimer >= _searchMoveTime || _agent.remainingDistance < .5f)
            {
                MoveToRandom();
                _currentSearchTimer = 0f;
            }
        }
    }
    private void UpdateCollect()
    {
        if (!_collecting)
        {
            StartCoroutine(CollectCurrentSphere());
        }
        //if (_stateTimer > 10f)
        //    ChangeState(AIState.Search);
    }

    private bool FindClosestSphere(float radius)
    {
        Transform closestSphere = null;
        float closestDistance = Mathf.Infinity;
        foreach (Transform sphere in _availableSpheres)
        {
            if (sphere == null || !sphere.gameObject.activeSelf) continue;

            float distance = Vector3.Distance(transform.position, sphere.position);

            //NavMeshPath path = new NavMeshPath();
            //if (_agent.CalculatePath(sphere.position, path))
            //{
                if (distance <= radius && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSphere = sphere;
                }
            //}
        }
        if (closestSphere != null)
        {
            _currentTargetSphere = closestSphere;
            return true;
        }
        return false;
    }

    private IEnumerator CollectCurrentSphere()
    {
        _collecting = true;
        _agent.isStopped = true;
        if (_animator != null)
        {
            SetAnimation("Collect");
            float collectDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(collectDuration);
        }
        else
        {
            yield return new WaitForSeconds(.7f);
        }
        if (_currentTargetSphere != null)
        {
            _availableSpheres.Remove(_currentTargetSphere);
            _currentTargetSphere.gameObject.SetActive(false);
            _currentTargetSphere = null;
        }
        _collecting = false;
        ChangeState(AIState.Idle);
    }
    private void ChangeState(AIState newState)
    {
        if (_currentState == newState) return;
        _currentState = newState;
        _stateTimer = 0f;
        _collecting = false;

        switch (newState)
        {
            case AIState.Idle:
                _agent.isStopped = true;
                SetAnimation("Idle");
                break;
            case AIState.Search:
                _agent.isStopped = false;
                SetAnimation("Search");
                //MoveToRandom();
                break;
            case AIState.Collect:
                //_agent.isStopped = false;
                //SetAnimation("Search");
                break;
        }
    }
    private void OnFootstep()
    { }
    private void OnLand()
    { }
    private void SetAnimation(string name)
    {
        if (_animator == null) return;
        _animator.ResetTrigger("Idle");
        _animator.ResetTrigger("Search");
        _animator.ResetTrigger("Collect");
        _animator.SetTrigger(name);
    }
    private void MoveToRandom()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        randomDirection.y += transform.position.y;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            _searchDestination = hit.position;
            _agent.SetDestination(_searchDestination);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
        if (_currentTargetSphere != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _currentTargetSphere.position);
            Gizmos.DrawWireSphere(_currentTargetSphere.position, 0.5f);
        }

        Gizmos.color = Color.yellow;
        foreach (Transform sphere in _availableSpheres)
        {
            if (sphere != null && sphere.gameObject.activeSelf)
            {
                Gizmos.DrawWireSphere(sphere.position, 0.3f);
            }
        }
        //if (_currentState == AIState.Search && _agent.hasPath)
        //{
        //    Gizmos.color = Color.white;
        //    Gizmos.DrawLine(transform.position, _agent.destination);
        //    Gizmos.DrawSphere(_agent.destination, .2f);
        //}
    }

    //public void AddSphere(Transform newSphere)
    //{
    //    if (!_allSpheres.Contains(newSphere))
    //    {
    //        _allSpheres.Add(newSphere);
    //        _availableSpheres.Add(newSphere);
    //    }
    //}
}