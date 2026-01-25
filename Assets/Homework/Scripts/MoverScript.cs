using UnityEngine;
using System.Collections;

public class MoverScript : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private float sensorDistance = 0.8f;
    [SerializeField] private LayerMask obstacleLayer;

    private bool isTurning = false;
    private float halfLength;
    private float halfWidth;
    private float nextRandomTurnTime;

    private void Start()
    {
        halfLength = transform.localScale.z * 0.5f;
        halfWidth = transform.localScale.x * 0.5f;
        nextRandomTurnTime = Time.time + Random.Range(3f, 8f);
    }

    private void Update()
    {
        if (!isTurning && Time.time >= nextRandomTurnTime)
        {
            StartCoroutine(RandomTurn90());
            nextRandomTurnTime = Time.time + Random.Range(5f, 10f);
        }
        if (isTurning) return;

        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        if (CheckFrontObstacle())
        {
            StartCoroutine(FindAndTurn());
        }
    }

    private bool CheckFrontObstacle()
    {
        Vector3 frontCenter = transform.position + transform.forward * halfLength;

        Ray centerRay = new Ray(frontCenter, transform.forward);
        Ray leftRay = new Ray(frontCenter + transform.right * -halfWidth, transform.forward);
        Ray rightRay = new Ray(frontCenter + transform.right * halfWidth, transform.forward);

        bool centerHit = Physics.Raycast(centerRay, sensorDistance, obstacleLayer);
        bool leftHit = Physics.Raycast(leftRay, sensorDistance, obstacleLayer);
        bool rightHit = Physics.Raycast(rightRay, sensorDistance, obstacleLayer);

        Debug.DrawRay(centerRay.origin, centerRay.direction * sensorDistance,
                     centerHit ? Color.red : Color.green);
        Debug.DrawRay(leftRay.origin, leftRay.direction * sensorDistance,
                     leftHit ? Color.red : Color.yellow);
        Debug.DrawRay(rightRay.origin, rightRay.direction * sensorDistance,
                     rightHit ? Color.red : Color.yellow);

        return centerHit || leftHit || rightHit;
    }
    private IEnumerator FindAndTurn()
    {
        isTurning = true;

        yield return new WaitForSeconds(0.1f);
        float bestAngle = FindBestTurnAngle();
        if (bestAngle != 0)
        {
            yield return StartCoroutine(ExecuteTurn(bestAngle));
        }
        else
        {
            yield return StartCoroutine(BackOutAndTurn());
        }
        yield return new WaitForSeconds(0.2f);
        isTurning = false;
    }

    private float FindBestTurnAngle()
    {
        bool checkLeftFirst = Random.Range(0, 2) == 0;

        if (checkLeftFirst)
        {
            if (CheckDirectionClear(-90f)) return -90f;
            if (CheckDirectionClear(90f)) return 90f;
        }
        else
        {
            if (CheckDirectionClear(90f)) return 90f;
            if (CheckDirectionClear(-90f)) return -90f;
        }

        if (CheckDirectionClear(180f)) return 180f;

        return 0f;
    }

    private bool CheckDirectionClear(float turnAngle)
    {
        Vector3 testDirection = Quaternion.Euler(0, turnAngle, 0) * transform.forward;
        Vector3 testFrontPoint = transform.position + testDirection * halfLength;
        Ray centerRay = new Ray(testFrontPoint, testDirection);
        Ray leftRay = new Ray(testFrontPoint + Quaternion.Euler(0, turnAngle, 0) * transform.right * -halfWidth, testDirection);
        Ray rightRay = new Ray(testFrontPoint + Quaternion.Euler(0, turnAngle, 0) * transform.right * halfWidth, testDirection);

        bool centerClear = !Physics.Raycast(centerRay, sensorDistance * 1.5f, obstacleLayer);
        bool leftClear = !Physics.Raycast(leftRay, sensorDistance, obstacleLayer);
        bool rightClear = !Physics.Raycast(rightRay, sensorDistance, obstacleLayer);

        if (isTurning)
        {
            Debug.DrawRay(centerRay.origin, centerRay.direction * sensorDistance * 1.5f,
                         centerClear ? Color.blue : Color.gray, 0.5f);
        }

        return centerClear && leftClear && rightClear;
    }

    private IEnumerator ExecuteTurn(float turnAngle)
    {
        //Debug.Log($"Turning: {turnAngle}°");

        float startAngle = transform.eulerAngles.y;
        float rawTarget = startAngle + turnAngle;
        float targetAngle = Mathf.Round(rawTarget / 90f) * 90f;
        targetAngle = (targetAngle % 360f + 360f) % 360f;

        float duration = Mathf.Abs(turnAngle) / turnSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, t);
            float snappedAngle = SnapTo90Degrees(currentAngle);
            transform.rotation = Quaternion.Euler(0, snappedAngle, 0);

            yield return null;
        }

        float finalAngle = SnapTo90Degrees(transform.eulerAngles.y);
        transform.rotation = Quaternion.Euler(0, finalAngle, 0);
    }

    private float SnapTo90Degrees(float angle)
    {
        angle = (angle % 360f + 360f) % 360f;
        float remainder = angle % 90f;

        if (remainder > 45f)
            return Mathf.Ceil(angle / 90f) * 90f;
        else
            return Mathf.Floor(angle / 90f) * 90f;
    }

    private IEnumerator BackOutAndTurn()
    {
        Debug.Log("No clear path - backing out");

        float backTime = 0.8f;
        float elapsed = 0f;

        while (elapsed < backTime)
        {
            elapsed += Time.deltaTime;
            transform.Translate(-Vector3.forward * speed * 0.5f * Time.deltaTime, Space.Self);
            yield return null;
        }

        float bestAngle = FindBestTurnAngle();
        if (bestAngle != 0)
        {
            yield return StartCoroutine(ExecuteTurn(bestAngle));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0 && !isTurning)
        {
            Debug.Log($"Collision: {collision.gameObject.name}");

            Vector3 pushDir = (transform.position - collision.contacts[0].point).normalized;
            transform.position += pushDir * 0.3f;

            transform.Rotate(0, 180f, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Vector3 frontCenter = transform.position + transform.forward * halfLength;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(frontCenter, 0.1f);
        Gizmos.DrawSphere(frontCenter + transform.right* -halfWidth, 0.08f);
        Gizmos.DrawSphere(frontCenter + transform.right* halfWidth, 0.08f);
    }
    private IEnumerator RandomTurn90()
    {
        isTurning = true;
        yield return new WaitForSeconds(0.1f);
        float turnAngle = Random.Range(0, 2) == 0 ? -90f : 90f;
        yield return StartCoroutine(ExecuteTurn(turnAngle));
        yield return new WaitForSeconds(0.2f);
        isTurning=false;
    }
}