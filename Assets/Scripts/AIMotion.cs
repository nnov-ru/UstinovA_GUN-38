using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public partial class AIMotion : MonoBehaviour
{
    [SerializeField] private List<Transform> _allSpheres = new List<Transform>();
    [SerializeField] private float _pickupDistance = 1.5f;
    [SerializeField] private float _searchRadius = 5f;
    [SerializeField] private float _idleTime = 5f;
    [SerializeField] private float _searchMoveTime = 3f;

    private NavMeshAgent _agent;
    private List<Transform> _availableSpheres;
    private Animator _animator;
    public NavMeshAgent agent => _agent;
    public Animator animator => _animator;
    public float stateTimer {  get => _stateTimer; set => _stateTimer = value; }
    public float idleTime => _idleTime;
    public List<Transform> availableSpheres => _availableSpheres;
    public float pickupDistance => _pickupDistance;
    public float searchMoveTime => _searchMoveTime;
    public float searchRadius => _searchRadius;

    private Idle _idle;
    private Search _search;
    private Collect _collect;
    private AIState _currentState = AIState.Search;
    private float _stateTimer = 0f;


    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _idle = new Idle(this);
        _search = new Search(this);
        _collect = new Collect(this);
        if (_allSpheres.Count == 0)
        {
            Debug.Log($"No balls in list in Inspector !");
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
                _idle.Update();
                break;
            case AIState.Search:
                _search.Update();
                break;
            case AIState.Collect:
                if (!_collect.collecting)
                    _collect.Update();
                break;
        }
    }
    public void ChangeState(AIState newState, object data = null)
    {
        if (_currentState == newState) return;
        _currentState = newState;
        _stateTimer = 0f;

        switch (newState)
        {
            case AIState.Idle:
                _agent.isStopped = true;
                SetAnimation("Idle");
                break;
            case AIState.Search:
                _agent.isStopped = false;
                SetAnimation("Search");
                _search?.Enter();
                break;
            case AIState.Collect:
                if (data is Transform closestSphere)
                    _collect.Enter(closestSphere);
                break;
        }
    }
    private void OnFootstep()
    { }
    private void OnLand()
    { }
    public void SetAnimation(string name)
    {
        if (_animator == null) return;
        _animator.ResetTrigger("Idle");
        _animator.ResetTrigger("Search");
        _animator.ResetTrigger("Collect");
        _animator.Play(name, 0, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
        if (_search.closestSphere != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _search.closestSphere.position);
            Gizmos.DrawWireSphere(_search.closestSphere.position, 0.5f);
        }

        Gizmos.color = Color.yellow;
        foreach (Transform sphere in _availableSpheres)
        {
            if (sphere != null && sphere.gameObject.activeSelf)
            {
                Gizmos.DrawWireSphere(sphere.position, 0.3f);
            }
        }
    }
}