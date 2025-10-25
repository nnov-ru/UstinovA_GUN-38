using TMPro;
using UnityEngine;
using Zenject;

namespace Unity3D
{
    public class TurnIndicator : MonoBehaviour
    {
        private ITurn _turn;  //injected
        private (Team team, TextMeshProUGUI icon) _left;
        private (Team team, TextMeshProUGUI icon) _right;

        [SerializeField]
        private GameObject _leftObject;
        [SerializeField]
        private GameObject _rightObject;
        [SerializeField, Range(.1f, 2f)]
        private float _disableScale = .7f;
        [SerializeField, Range(0f, 1f)]
        private float _disableAlpha = .3f;

        [Inject]
        private void Construct(SignalBus signal, ITurn turn)
        {
            if (_leftObject == null || _rightObject == null)
            { 
                Debug.LogError("Objects arent assigned to TurnIndicator", this);
                return;
            }
            _turn = turn;
            var oneByOneTurn = turn as OneByOneTurn;
            if (oneByOneTurn == null) return;

            _left = (oneByOneTurn.White, _leftObject.GetComponent<TextMeshProUGUI>());
            _right = (oneByOneTurn.Black, _rightObject.GetComponent<TextMeshProUGUI>());

            _left.icon.text = $"Player {(int)_left.team + 1}";
            _right.icon.text = $"Player {(int)_right.team + 1}";

            _left.icon.transform.localScale = Vector3.one;
            _left.icon.color = new Color(1f, 1f, 1f, 1f);
            _right.icon.transform.localScale = Vector3.one * _disableScale;
            _right.icon.color = new Color(1f, 1f, 1f, _disableAlpha);

            signal.Subscribe<GameStatus>(Callback);
        }
        private void Callback(GameStatus status)
        {
            if (status is not GameStatus.Locked) return;
            if (_turn == null) return;

            _turn.Next();
            var (active, inactive) = _left.team == _turn.Current
                ? (_left, _right)
                : (_right, _left);
            active.icon.transform.localScale = Vector3.one;
            active.icon.color = new Color(1f, 1f, 1f, 1f);

            inactive.icon.transform.localScale = Vector3.one * _disableScale;
            inactive.icon.color = new Color(1f, 1f, 1f, _disableAlpha);
        }
    }
}
