using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AIMotion : MonoBehaviour
{
    [SerializeField] private List<Transform> _allSpheres = new List<Transform>(); // Все 8 сфер
    [SerializeField] private float _pickupDistance = 1.5f;
    [SerializeField] private bool _debugMode = true;
    //[SerializeField] private float _maxTime;

    //private float _timer;
    private NavMeshAgent _agent;
    private Transform _currentTargetSphere;
    private List<Transform> _availableSpheres = new List<Transform>(); // Доступные сферы
    private Animator _animator;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        //_timer = _maxTime;
        if (_allSpheres.Count == 0)
        {
            Debug.LogError("Нет сфер в списке! Добавьте сферы в инспекторе.");
            return;
        }

        _availableSpheres = new List<Transform>(_allSpheres);

        FindAndMoveToClosestSphere();
    }

    private void Update()
    {
        //_timer -= Time.deltaTime;
        //if (_timer <= 0)
        //{
            if (_currentTargetSphere == null || _availableSpheres.Count == 0)
            {
                if (_availableSpheres.Count > 0)
                {
                    FindAndMoveToClosestSphere();
                }
                else
                {
                    Debug.Log("Все сферы собраны!");
                    _agent.isStopped = true;
                    return;
                }
            }

            if (_currentTargetSphere != null)
            {
                float distance = Vector3.Distance(transform.position, _currentTargetSphere.position);

                if (distance <= _pickupDistance)
                {
                    CollectCurrentSphere();
                }

                if (_debugMode && Time.frameCount % 30 == 0)
                {
                    Debug.Log($"Цель: {_currentTargetSphere.name}, Расстояние: {distance:F2}");
                }
            }

        _animator.SetFloat("Speed", _agent.velocity.magnitude);
        //}
    }

    private void FindAndMoveToClosestSphere()
    {
        if (_availableSpheres.Count == 0) return;

        Transform closestSphere = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform sphere in _availableSpheres)
        {
            if (sphere == null) continue;

            float distance = Vector3.Distance(transform.position, sphere.position);

            NavMeshPath path = new NavMeshPath();
            if (_agent.CalculatePath(sphere.position, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSphere = sphere;
                }
            }
        }

        if (closestSphere != null)
        {
            _currentTargetSphere = closestSphere;
            _agent.SetDestination(_currentTargetSphere.position);

            if (_debugMode)
                Debug.Log($"Новая цель: {_currentTargetSphere.name}");
        }
        else
        {
            Debug.LogWarning("Нет доступных сфер!");
        }
    }

    private void CollectCurrentSphere()
    {
        if (_currentTargetSphere == null) return;

        if (_debugMode)
            Debug.Log($"Сфера собрана: {_currentTargetSphere.name}");

        _availableSpheres.Remove(_currentTargetSphere);

        _currentTargetSphere.gameObject.SetActive(false);

        _currentTargetSphere = null;

        if (_availableSpheres.Count > 0)
        {
            FindAndMoveToClosestSphere();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_currentTargetSphere != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _currentTargetSphere.position);
            Gizmos.DrawWireSphere(_currentTargetSphere.position, 0.5f);
        }

        Gizmos.color = Color.yellow;
        foreach (Transform sphere in _availableSpheres)
        {
            if (sphere != null)
            {
                Gizmos.DrawWireSphere(sphere.position, 0.3f);
            }
        }
    }

    public void AddSphere(Transform newSphere)
    {
        if (!_allSpheres.Contains(newSphere))
        {
            _allSpheres.Add(newSphere);
            _availableSpheres.Add(newSphere);

            if (_currentTargetSphere == null)
            {
                _currentTargetSphere = newSphere;
                _agent.SetDestination(newSphere.position);
            }
        }
    }
}