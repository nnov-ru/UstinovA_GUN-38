using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using Zenject;
using System.Collections;
using System;

namespace Unity3D
{
    public class InputManager : MonoBehaviour
    {
        private Controls.GameActions _controls;
        private ISharedData _data;

        private Coroutine _restartCoroutine;

        [SerializeField]
        private GameObject _restartUI;
        [SerializeField]
        private Image _restartFill;
        [SerializeField, Range(0.1f, 1f)]
        private float _restartHoldTime = 0.25f;
        [SerializeField]
        private GameObject _confirmUI;
        [SerializeField]
        private Image _confirmFill;
        [SerializeField, Range(0.01f, 1f)]
        private float _confirmlAnimatTime = 1f;
        [SerializeField]
        private GameObject _cancelUI;
        [SerializeField]
        private Image _cancelFill;
        [SerializeField, Range(0.01f, 1f)]
        private float _cancelAnimatTime = 1f;
        [Inject]
        private void Construct(Controls.GameActions controls)
        {
            _controls = controls;
        }

        #region Restart Logic
        public event Action OnRestartPerformed;
        private void OnRestart(InputAction.CallbackContext obj)
        {
            _restartUI.gameObject.SetActive(true);
            _restartCoroutine = StartCoroutine(Restarter());
        }
        private void OnRestartCanceled(InputAction.CallbackContext obj)
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
            OnRestartPerformed?.Invoke();
            ResetRestartI();
            //var index = SceneManager.GetActiveScene().buildIndex;
            //SceneManager.LoadScene(index);
        }
        private void ResetRestartI()
        {
            _restartFill.fillAmount = 0f;
            _restartUI.gameObject.SetActive(false);
        }
        #endregion
        #region Cancel Logic
        public event Action OnCancelPressed;
        private void OnCancel(InputAction.CallbackContext obj)
        {
            OnCancelPressed?.Invoke();
            StartCoroutine(Canceller());
        }
        private IEnumerator Canceller()
        {
            float timer = 0f;
            while (timer < _cancelAnimatTime)
            {
                timer += Time.deltaTime;
                float fillAmount = timer / _cancelAnimatTime;
                _cancelFill.fillAmount = fillAmount;
                yield return null;
            }
            _cancelFill.fillAmount = 1f;
        }
        #endregion
        #region Confirm Logic
        public event Action OnConfirmPressed;
        private void OnConfirm(InputAction.CallbackContext obj)
        {
                OnConfirmPressed?.Invoke();
                StartCoroutine(Confirmer());
        }
        private IEnumerator Confirmer()
        {
            //if (_data.Destination == null) yield return null;
            float timer = 0f;
            while (timer < _confirmlAnimatTime)
            {
                timer += Time.deltaTime;
                float fillAmount = timer / _confirmlAnimatTime;
                _confirmFill.fillAmount = fillAmount;
                yield return null;
            }
            _confirmFill.fillAmount = 1f;
        }
        #endregion

        private void Start()
        {
            _controls.Restart.performed += OnRestart;
            _controls.Restart.canceled += OnRestartCanceled;
            if (_controls.Cancel != null)
                _controls.Cancel.performed += OnCancel;
            if (_controls.Confirm != null)
                _controls.Confirm.performed += OnConfirm;
            ResetRestartI();
        }
        private void OnDestroy()
        {
            _controls.Restart.performed -= OnRestart;
            _controls.Restart.canceled -= OnRestartCanceled;

            if (_controls.Cancel != null)
                _controls.Cancel.performed -= OnCancel;
            if (_controls.Confirm != null)
                _controls.Confirm.performed -= OnConfirm;
        }
        
    }
}