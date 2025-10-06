using System.Collections;
using UnityEngine;
using UnityEngine.WSA;

namespace CoroutineHomework
{
	public class Player : MonoBehaviour
	{
		private bool _ready;
		private Rigidbody _ball;
		
		[SerializeField]
		private Rigidbody _ballPrefab;
		[SerializeField]
		private float _startVelocity;
		[SerializeField]
		private float _lifetime;

		[SerializeField]
		private float _respawnDelay;

		private void Start()
		{
            _ready = true;
            Spawn();
		}

		private void Spawn()
		{
            _ball = Instantiate(_ballPrefab, transform);
			_ball.isKinematic = true;
            _ball.transform.parent = transform;
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
			_ball.velocity = transform.forward * _startVelocity;
            if (_ball != null)
			{
				Destroy(_ball.gameObject, _lifetime);
			}
			_ball = null;
		}

		private void Update()
		{
			if (!_ready) return;
            if (Input.GetKeyDown(KeyCode.Space) && _ball != null)
			{
				LaunchReadyBall();
				StartCoroutine(Reloader());
			}
		}
	}
}