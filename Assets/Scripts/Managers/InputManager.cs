using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using Zenject;
using System.Collections;
using TMPro;

namespace Unity3D
{
    public class InputManager : MonoBehaviour
    {
        private SceneController _controller;
        private Controls.GameActions _controls;

        private Coroutine _restartCoroutine;

        [SerializeField]
        private GameObject _restartUI;
        [SerializeField]
        private Image _restartFill;
        [SerializeField]
        private TextMeshProUGUI _restartText;
        [SerializeField, Range(0.1f, 1f)]
        private float _restartHoldTime = 0.25f;

        private void OnRestartPerformed(InputAction.CallbackContext obj)
        {
            _restartUI.gameObject.SetActive(true);
            _restartCoroutine = StartCoroutine(Restarter());
        }
        private void OnRestartCancelled(InputAction.CallbackContext obj)
        {
            if (_restartCoroutine != null) StopCoroutine(_restartCoroutine);
            ResetRestartI();
        }
        private IEnumerator Restarter()
        {
            float timer = 0f;
            while (timer < _restartHoldTime)
            {
                timer += Time.deltaTime;
                float fillAmount = timer / _restartHoldTime;
                _restartFill.fillAmount = fillAmount;
                yield return null;
            }
            ResetRestartI();
            _controller.OpenGameScene();
        }
        private void ResetRestartI()
        {
            _restartFill.fillAmount = 0f;
            _restartUI.gameObject.SetActive(false);
        }
        private void Start()
        {
            _controls.Restart.performed += OnRestartPerformed;
            _controls.Restart.canceled += OnRestartCancelled;
            ResetRestartI();
        }
        private void OnDestroy()
        {
            _controls.Restart.performed -= OnRestartPerformed;
            _controls.Restart.canceled -= OnRestartCancelled;
        }
        //??
        [Inject]
        private void Construct(SceneController controller, Controls.GameActions controls)
        {
            _controller = controller;
            _controls = controls;
        }
    }
}
