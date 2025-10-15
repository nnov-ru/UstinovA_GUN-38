using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity3D
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField]
        private MeshRenderer _focus;
        [SerializeField]
        private MeshRenderer _select;
        public Unit Unit { get; set; }

        public event Action<Cell> OnPointerClickEvent;
        public void OnPointerEnter (PointerEventData eventData)
        {
            _focus.enabled = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _focus.enabled = false;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickEvent?.Invoke(this);
        }
        public void SetSelect(Material material)
        {
            _select.enabled = true;
            _select.sharedMaterial = material;
        }
        //cancel selection
        public void ResetSelect()
        {
            _select.enabled = false;
        }
    }
}
