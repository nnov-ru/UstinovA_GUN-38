using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverScript : MonoBehaviour
{
    public enum CoordinatesMode { Global, Local }

    [SerializeField] private CoordinatesMode _coordinatesMode = CoordinatesMode.Global;
    [SerializeField] private Vector3 _start;
    [SerializeField] private Vector3 _end;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _delay = 1f;

    private Rigidbody _rigidbody;
    private Vector3 _initialPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialPosition = _rigidbody.position;
        StartCoroutine(MovementCoroutine());
    }

    private IEnumerator MovementCoroutine()
    {
        yield return new WaitForFixedUpdate();
        Vector3 start = GetPosition(_start);
        Vector3 end = GetPosition(_end);

        while (true)
        {
            yield return StartCoroutine(MoveBetween(start, end));
            yield return new WaitForSeconds(_delay);
            yield return StartCoroutine(MoveBetween(end, start));
            yield return new WaitForSeconds(_delay);
        } 
    }
    private Vector3 GetPosition(Vector3 inputPosition)
    {
        return _coordinatesMode == CoordinatesMode.Local
            ? _initialPosition + inputPosition
            : inputPosition;
            }
    private IEnumerator MoveBetween(Vector3 from, Vector3 to)
    { 
            while (Vector3.Distance(_rigidbody.position, to) > 0f)
            {
                    Vector3 newPosition = Vector3.MoveTowards(
                        _rigidbody.position, 
                        to,
                        _speed * Time.fixedDeltaTime
                        );

                    _rigidbody.MovePosition( newPosition );
                    yield return new WaitForFixedUpdate();
            }
            _rigidbody.MovePosition(to);
    }
    private void OnDrawGizmos()
    {
        Vector3 start = GetPositionGiz(_start);
        Vector3 end = GetPositionGiz(_end);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(start, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(end, 0.5f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, end);
    }
    private Vector3 GetPositionGiz(Vector3 inputPosition)
    {
        if (Application.isPlaying)
        {
            return GetPosition(inputPosition);
        }
        else
        {
            return _coordinatesMode == CoordinatesMode.Local
                ? transform.position + inputPosition 
                : inputPosition;
        }
    }
#if UNITY_EDITOR
    [ContextMenu("Switch Coordinates Mode")]
    private void SwitchCoordinatesMode()
    {
        if (_coordinatesMode == CoordinatesMode.Local)
        {
            Vector3 currentWorldPos = Application.isPlaying ? _initialPosition : transform.position;
            _start = currentWorldPos + _start;
            _end = currentWorldPos + _end;
            _coordinatesMode = CoordinatesMode.Global;
        }
        else
        {
            Vector3 currentWorldPos = Application.isPlaying ? _initialPosition : transform.position;
            _start = _start - currentWorldPos;
            _end = _end - currentWorldPos;
            _coordinatesMode = CoordinatesMode.Local;
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
