using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotate;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(RotationCoroutine());
    }
    private IEnumerator RotationCoroutine()
    {
        yield return new WaitForFixedUpdate();
        while (true)
        {
            if (_rigidbody != null)
            {
                Quaternion deltaRotation = Quaternion.Euler(_rotate * (Time.fixedDeltaTime));
                _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
