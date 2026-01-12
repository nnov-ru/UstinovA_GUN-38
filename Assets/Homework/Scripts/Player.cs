using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	[System.Serializable]
	public class BallType
	{
		public string name;
		public float size = 1f;
		public float mass = 1f;
		public float startVelocity = 15f;
		public Color color = Color.white;
	}
	private bool _ready;
	private Rigidbody _ball;
	private Gates _gates;

	[SerializeField]
	private BallType[] _ballTypes;
	[SerializeField]
	private int _selectedBallType = 0;
	[SerializeField]
	private Rigidbody _ballPrefab;
	[SerializeField]
	private float _lifetime = 15f;

	[SerializeField]
	private float _respawnDelay = 4f;
	[SerializeField]
	private Transform _respawnPoint;

	private void Start()
	{
		_gates = FindObjectOfType<Gates>();
		_ready = true;
		if (_respawnPoint == null) _respawnPoint = transform;
		Spawn();
	}

	private void Spawn()
	{
		if (_ballPrefab == null) return;
		_ball = Instantiate(_ballPrefab, _respawnPoint.position, _respawnPoint.rotation);
		_ball.isKinematic = true;
		_ball.transform.parent = transform;

		if (_ball == null || _selectedBallType < 0 || _selectedBallType >= _ballTypes.Length) return;
		BallType type = _ballTypes[_selectedBallType];
		_ball.transform.localScale = Vector3.one * type.size;
		_ball.mass = type.mass;
		MeshRenderer renderer = _ball.GetComponent<MeshRenderer>();
		if (renderer != null ) renderer.material.color = type.color;
	}

	private IEnumerator Reloader()
	{
		_ready = false;
		yield return new WaitForSeconds(_respawnDelay);
		Spawn();
		_ready = true;
	}
	private void LaunchReadyBall()
	{
		if (_ball == null) return;
		_ball.isKinematic = false;
		_ball.transform.parent = null;
		_ball.velocity = transform.forward * _ballTypes[_selectedBallType].startVelocity;
		if (_ball != null)
		{
			Destroy(_ball.gameObject, _lifetime);
		}
		if (_gates != null) _gates.StartStopTracking(_lifetime);
		_ball = null;
		StartCoroutine(Reloader());
	}

	private void Update()
	{
		if (!_ready) return;
		if (Input.GetMouseButtonDown(0) && _ball != null)
		{
			LaunchReadyBall();
			StartCoroutine(Reloader());
		}
		for (int i = 0; i < Mathf.Min(_ballTypes.Length, 9); i++)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1 + i))
			{
				_selectedBallType = i;
				Debug.Log($"You chose Ball Type {_ballTypes[i].name}");
				if (_ball != null)
				{
                    Destroy(_ball.gameObject);
                    Spawn();
				}
			}
		}
		//if (Input.GetKeyDown(KeyCode.R))
		//{
			//Gates gates = FindObjectOfType<Gates>();
			//if (gates != null) gates.ResetGame();
		//}
	}
}