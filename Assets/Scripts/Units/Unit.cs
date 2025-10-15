using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity3D
{
    public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private float _moveSpeed = 5f;
        private Cell _currentCell;
        private bool _isMoving = false;
        public Cell Cell 
        { 
            get => _currentCell; 
            set => _currentCell = value; 
        }
        public event Action<Unit> OnMoveEndCallback;
        public void OnPointerEnter(PointerEventData eventData)
        {
            _currentCell?.OnPointerEnter(eventData);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _currentCell?.OnPointerExit(eventData);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            _currentCell?.OnPointerClick(eventData);
        }
        public void Move(Cell cell)
        {
            if (_isMoving || cell == null || cell == _currentCell) 
                return;
            StartCoroutine(MoveCoroutine(cell));
        }
        private IEnumerator MoveCoroutine(Cell cell)
        { 
            _isMoving = true;

            Vector3 startPosition = transform.position;
            Vector3 endPosition = cell.transform.position;

            endPosition.y = startPosition.y;

            float pathLength = Vector3.Distance(startPosition, endPosition);
            float startTime = Time.time;

            while (transform.position != endPosition)
            {
                float distanceDone = (Time.time - startTime) * _moveSpeed;
                float pathPart = distanceDone / pathLength;
                transform.position = Vector3.Lerp(startPosition, endPosition, pathPart);
                yield return null;
            }

            if (_currentCell != null)
                _currentCell.Unit = null;
            _currentCell = cell;
            cell.Unit = this;
            _isMoving = false;
            OnMoveEndCallback?.Invoke(this);
        }
        private void OnDestroy()
        {
            if (_currentCell != null && _currentCell.Unit == this)
            {
                _currentCell.Unit = null;
            }
        }
        public bool IsMoving => _isMoving;
    }
}