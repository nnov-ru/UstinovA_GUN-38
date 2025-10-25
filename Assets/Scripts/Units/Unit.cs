using UnityEngine;
using UnityEngine.EventSystems;
//имплементирует интерфейсы IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
//которые обеспечивают выделение юнита на выделенной клетке,
//также имеет ссылку на свою Cell
//и хранит данные, определ€ющие фигуру/фишку (еЄ тип, команду, статы и тд
namespace Unity3D
{
    public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private bool _isWhite;
        [SerializeField]
        private float _speed = 5f;
        [SerializeField]
        private float _jumpHeight = 1f;
        [SerializeField]
        private string _teamdisplay;
        [SerializeField]  //temp
        private string _queendisplay;  //temp
        [SerializeField]
        private Material _queenMaterial;
        private Material _initialMaterial;
        private Renderer _renderer;
        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _initialMaterial = GetComponent<Material>();
        }
        private void OnValidate()
        {
            _teamdisplay = Team.ToString();
            _queendisplay = IsQueen ? "Queen" : "Normal";
        }
        public bool IsWhite => _isWhite;
        public float Speed => _speed;
        public float JumpHeight => _jumpHeight;
        public Team Team { get; private set; }
        public bool IsQueen { get; private set; }
        public Cell Cell { get; set; }
        public bool IsMoving { get; set; }

        public void SetTeam (Team team) 
        { 
            Team = team; 
        }
        public void Promotion() 
        { 
            IsQueen = true; 
            _queendisplay = "Queen";
            if (_queenMaterial != null && _renderer != null)
            {
                _renderer.material = _queenMaterial;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            Cell?.OnPointerEnter(eventData);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            Cell?.OnPointerExit(eventData);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Cell == null)
            {
                Debug.LogError($"Unit {name}, has no cell reference");
                return;
            }
            Cell?.OnPointerClick(eventData);
        }
        private void OnDestroy()
        {
            if (Cell != null && Cell.Unit == this)
            {
                Cell.Unit = null;
            }
        }
    }
}