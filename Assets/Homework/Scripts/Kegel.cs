using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kegel : MonoBehaviour
{
    private bool _sent = false;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Rigidbody _body;
    public bool Sent => _sent;
    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
        _body = GetComponent<Rigidbody>();
    }

    public void MarkAsSent()
    {
        _sent = true;
    }
    public void ResetKegel()
    {
        _sent = false;
        if (_body != null)
        {
            _body.isKinematic = false;
            _body.velocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
        }
        transform.position = _initialPosition;
        transform.rotation = _initialRotation; 
    }
}
