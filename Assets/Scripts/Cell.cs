using System;
using UnityEngine;
using UnityEngine.EventSystems;
//имплементирует интерфейсы IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
//manages focus select deselect click events
//также имеет ссылку на Unita, стоящего на ней 
namespace Unity3D
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField]
        private MeshRenderer _focus;
        [SerializeField]
        private MeshRenderer _select;
        public Unit Unit { get; set; }
        public bool IsEmpty => Unit == null;
        public Vector2Int GridPosition { get; set; }
        private bool _isPointerOver = false;
        private bool IsValidPointerMovement(PointerEventData eventData)
        {
            if (eventData.delta.sqrMagnitude < 0.01f)
                return false;
            return true;
        }

        public event Action<Cell> OnPointerClickEvent;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsValidPointerMovement(eventData)) return;
            _isPointerOver = true;
            if (!_select.enabled)
            {
                _focus.enabled = true;
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _focus.enabled = false;
            _isPointerOver = false;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickEvent?.Invoke(this);
        }
        public void SetSelect(Material material)
        {
            _focus.enabled = false;
            _select.enabled = true;
            _select.sharedMaterial = material;
        }
        //cancel selection
        public void ResetSelect()
        {
            _select.enabled = false;
            if (_isPointerOver)
            {
                _focus.enabled = true;
            }
        }
    }
}
