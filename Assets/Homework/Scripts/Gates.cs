using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField] private int _totalScore = 0;
    [SerializeField] private int _currentScore = 0;
    [SerializeField] private int _currentFrame = 1;
    [SerializeField] private int _throwNumber = 1;
    [SerializeField] private int _remainingKegeln = 10;
    [SerializeField] private int _maxFrames = 3;

    private List<FrameData> _frames = new List<FrameData> ();
    private int _bonusCount = 0;
    private int _bonus = 0;
    private List<GameObject> _kegeln = new List<GameObject>();
    private List<Vector3> _kegelPositions = new List<Vector3>();
    private List<Quaternion> _kegelRotations = new List<Quaternion>();

    private bool _isTracking = false;
    private Coroutine _trackingCoroutine;
    private List<Rigidbody> _trackedBodies = new List<Rigidbody>();
    private Player _player;
    private class FrameData
    {
        public int firstThrow = 0;
        public int secondThrow = 0;
        public bool isStrike = false;
        public bool isSpare = false;
        public int frameScore = 0;
        public bool scoreCalculated = false;
    }
    private void Start()
    {
        InitializeFrames();
        _kegeln.Clear();
        _kegelPositions.Clear();
        _kegelRotations.Clear();
        _player = FindObjectOfType<Player>();
        GameObject[] remainingKegeln = GameObject.FindGameObjectsWithTag("Kegel");
        foreach (GameObject kegel in remainingKegeln)
            {
                if (kegel != null)
                {
                    _kegeln.Add(kegel);
                    _kegelPositions.Add(kegel.transform.position);
                    _kegelRotations.Add(kegel.transform.rotation);
                }
            }
        _remainingKegeln = _kegeln.Count;
        if (_kegeln.Count == 0) print("No kegeln found, please beware");
        Debug.Log($"{_remainingKegeln} kegeln are to be sent to the Gates");
    }
    private void InitializeFrames()
    {
        _frames.Clear();
        for (int i = 0; i < _maxFrames; i++)
        {
            _frames.Add(new FrameData());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            Destroy(other.gameObject);
            return;
        }
        Kegel kegel = other.GetComponent<Kegel>();
        if (kegel != null && !kegel.Sent)
        {
            kegel.MarkAsSent();
            other.gameObject.SetActive(false);
            _currentScore++;
            _remainingKegeln--;
            if (_bonusCount > 0) _bonus++;
        }
    }
    public void StartStopTracking(float ballLifetime)
    {
        if (_isTracking) return;
        _isTracking = true;
        if (_trackingCoroutine != null) StopCoroutine( _trackingCoroutine );
        _trackingCoroutine = StartCoroutine(TrackingSequence(ballLifetime));
    }
    private IEnumerator TrackingSequence(float ballLifetime)
    {
        float wait = 2f;
        if (_player != null) wait = ballLifetime;
        yield return new WaitForSeconds(wait);
        CollectAllMovingBodies();
        yield return StartCoroutine(WaitForStop());
        CalculateTotalScore();
    }
    private void CollectAllMovingBodies()
    {
        _trackedBodies.Clear();
        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody body in allRigidbodies)
        {
            if (body != null && body.gameObject.activeInHierarchy && !body.isKinematic)
            {
                _trackedBodies.Add(body);
            }
        }
    }
    private IEnumerator WaitForStop()
    {
        bool allStopped = false;
        float stoppedTime = 0f;
        const float requiredStopTime = 1f;
        float maxWait = 15f;
        float pastTime = 0f;
        while ((!allStopped || stoppedTime < requiredStopTime) && pastTime < maxWait)
        {
            allStopped = CheckIfAllStopped();
            if (allStopped ) stoppedTime += Time.deltaTime;
            else stoppedTime = 0f;
            pastTime += Time.deltaTime;
            yield return null;
        }
    }
    private bool CheckIfAllStopped()
    {
        _trackedBodies.RemoveAll(body => body == null);
        if (_trackedBodies.Count == 0) return true;
        foreach (Rigidbody body in _trackedBodies)
        {
            if (body == null) continue;
            if (body.velocity.magnitude > .01f || body.angularVelocity.magnitude > .01f) return false;
        }
        return true;
    }
    private void CalculateTotalScore()
    {
        if (!_isTracking) return;
        SaveThrowResult();
        print($"Frame {_currentFrame}, throw {_throwNumber}: {_currentScore} points + {_bonus} bonuses gained");
        UpdateTotalScore();
        PrepareForNextThrow();
        _isTracking = false;
    }
    private void SaveThrowResult()
    {
        int frameIndex = _currentFrame - 1;
        if (_throwNumber == 1)
        {
            _frames[frameIndex].firstThrow = _currentScore + _bonus;
            if (_bonusCount != 0) _bonusCount--;
            if (_currentScore == 10)
            {
                _frames[frameIndex].isStrike = true;
                _bonusCount++;
                _bonusCount++;
            }
        }
        else if (_throwNumber == 2)
        {
            _frames[frameIndex].secondThrow = _currentScore + _bonus;
            if (_bonusCount != 0) _bonusCount--;
            if (_frames[frameIndex].firstThrow + _currentScore == 10)
            {
                _frames[frameIndex].isSpare = true;
                _bonusCount++;
            }
        }
    }
    private void UpdateTotalScore()
    {
        _totalScore = 0;
        for (int i = 0; i < _currentFrame; i++)
        {
            CalculateScore(i);
            _totalScore += _frames[i].frameScore;
        }
    }
    private void CalculateScore(int frameIndex)
    {
        var frame = _frames[frameIndex];
        frame.frameScore = frame.firstThrow + frame.secondThrow;
        frame.scoreCalculated = true;
        //Debug.Log($"Frame {frameIndex + 1}: You got {frame.frameScore}");
        if (frame.isStrike)
        {
            Debug.Log($"STRIKE! SCORE +10 + BONUS IN THE NEXT 2 THROWS = {frame.frameScore}");
        }
        else if (frame.isSpare)
        {
            Debug.Log($"SPARE! SCORE +10 + BONUS IN THE NEXT 1 THROW = {frame.frameScore}");
        }
        _currentScore = 0;
        _bonus = 0;
    }
    private void PrepareForNextThrow()
    {
        if (_remainingKegeln == 0 || _throwNumber == 2)
        {
            _currentFrame++;
            _throwNumber = 1;
            if (_currentFrame > _maxFrames)
            { 
                Debug.Log($"GAME OVER !!!!!!!!!!! FINAL SCORE : {_totalScore}");
                _bonusCount = 0;
                return;
            }
            _remainingKegeln = _kegeln.Count;
            ResetKegelnForNextFrame();
        }
        else
        {
            _throwNumber = 2;
            ResetRemainingForNextFrame();
        }
    }
    private void ResetRemainingForNextFrame()
    {
        for (int i = 0; i < _kegeln.Count; i++)
        {
            if (_kegeln[i] == null) continue;

            Kegel kegel = _kegeln[i].GetComponent<Kegel>();
            if (kegel != null && kegel.Sent) continue;
            Rigidbody body = _kegeln[i].GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.isKinematic = true;
            }
            _kegeln[i].transform.position = _kegelPositions[i];
            _kegeln[i].transform.rotation = _kegelRotations[i];
            if (kegel != null) kegel.ResetKegel();
            if (body != null) body.isKinematic = false;
        }
    }
    private void ResetKegelnForNextFrame()
    {
        for (int i = 0; i < _kegeln.Count; i++)
        {
            if (_kegeln[i] == null) continue;
            _kegeln[i].SetActive(true);
            Rigidbody body = _kegeln[i].GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.isKinematic = true;
            }
            _kegeln[i].transform.position = _kegelPositions[i];
            _kegeln[i].transform.rotation = _kegelRotations[i];

            Kegel kegel = _kegeln[i].GetComponent<Kegel>();
            if (kegel != null) kegel.ResetKegel();
            if (body != null) body.isKinematic = false;
        }
        _remainingKegeln = _kegeln.Count;
    }
    public void ResetGame()
    {
        _totalScore = 0;
        _currentScore = 0;
        _bonus = 0;
        _currentFrame = 1;
        _throwNumber = 1;
        _remainingKegeln = _kegeln.Count;
        _bonusCount = 0;
        InitializeFrames();

        if (_trackingCoroutine != null )
        {
            StopCoroutine(_trackingCoroutine);
            _isTracking = false;
        }
        ResetKegelnForNextFrame();
        Debug.Log($"Game has been reset");
    }
}